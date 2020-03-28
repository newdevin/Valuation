using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Valuation.Common;
using Valuation.Domain;

namespace Valuation.Service
{
    public class ValuationCalculator
    {
        public List<ListingValuation> GetValuations(DateTime upToDate, IEnumerable<ListingVolume> listingVolumes, IEnumerable<EndOfDayPrice> eodPrices, IEnumerable<CurrencyRate> currencyRates)
        {
            var distinctListings = listingVolumes.Select(lv => lv.Listing).GroupBy(l => l.Id).Select(g => g.First()).ToList();
            var listingIdsWithDate = distinctListings.SelectMany(l =>
            {
                var fromDate = listingVolumes
                                .Where(lv => lv.Listing.Id == l.Id)
                                .Min(lv => lv.Day);
                var range = DateTimeExtensions.GetDateRange(fromDate, upToDate);
                return range.Select(d => new { ListingId = l.Id, Day = d, Currency = l.Currency });
            }).ToList();

            var listingIdsWithDateAndQuantity = listingIdsWithDate
                .Select(l =>
                {
                    var listingVolume = listingVolumes
                            .Where(lv => lv.Listing.Id == l.ListingId && lv.Day <= l.Day)
                            .OrderByDescending(lv => lv.Day)
                            .FirstOrDefault();
                    return new { l.ListingId, l.Day, listingVolume.Quantity, l.Currency.Symbol };
                })
                .Where(p => p.Quantity > 0)
                .ToList();

            var valuations = listingIdsWithDateAndQuantity.AsParallel().Select(l =>
            {
                var sinceDay = listingVolumes.Where(lv => lv.Listing.Id == l.ListingId).Min(x => x.Day);
                var eodPrice = eodPrices.Where(eod => eod.ListingId == l.ListingId && eod.Day <= l.Day)
                                .OrderByDescending(x => x.Day)
                                .First();
                var rate = 1m;
                if (l.Symbol != "GBP")
                {
                    rate = currencyRates.Where(cr => cr.From == l.Symbol && cr.Day <= l.Day)
                    .OrderByDescending(x => x.Day)
                    .First()
                    .Rate;
                }

                return new ListingValuation
                {
                    Day = l.Day,
                    Currency = l.Symbol,
                    ListingId = l.ListingId,
                    Price = eodPrice.ClosePrice.Value,
                    Quantity = l.Quantity,
                    Rate = rate,
                    TotalValue = l.Quantity * eodPrice.ClosePrice.Value,
                    TotalValueInGbp = l.Quantity * eodPrice.ClosePrice.Value * rate
                };

            })
            .ToList();
            return valuations;
        }
    }
}
