using System;
using System.Linq;
using System.Threading.Tasks;
using Valuation.Common;
using Valuation.Domain;
using Valuation.Service.Repository;

namespace Valuation.Service
{
    public class ValuationService : IValuationService
    {
        private readonly IEndOfDayPriceService endOfDayPriceService;
        private readonly ICurrencyRateService currencyRateService;
        private readonly IListingService listingService;
        private readonly IValuationRepository valuationRepository;

        public ValuationService(IEndOfDayPriceService endOfDayPriceService,
            ICurrencyRateService currencyRateService,
            IListingService listingService,
            IValuationRepository valuationRepository)
        {
            this.endOfDayPriceService = endOfDayPriceService;
            this.currencyRateService = currencyRateService;
            this.listingService = listingService;
            this.valuationRepository = valuationRepository;
        }


        public async Task ValuePortfolio(DateTime upToDate)
        {
            //throw new NotImplementedException();
            var listingVolumes = await listingService.GetListingVolumes();
            var firstDay = listingVolumes.Min(lv => lv.Day);

            var distinctListings = listingVolumes.Select(lv => lv.Listing).GroupBy(l => l.Id).Select(g => g.First()).ToList();

            var eodPrices = await endOfDayPriceService.GetEndOfDayPriceSince(firstDay);
            var currencyRates = await currencyRateService.GetCurencyRatesSince(firstDay);

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
                    Price = eodPrice.ClosePrice.Value ,
                    Quantity = l.Quantity,
                    Rate = rate,
                    TotalValue = l.Quantity * eodPrice.ClosePrice.Value ,
                    TotalValueInGbp = l.Quantity * eodPrice.ClosePrice.Value * rate 
                };

            })
            .ToList();

             await valuationRepository.Save(valuations);
        }
    }

}

