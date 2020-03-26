using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuation.Common;


namespace Valuation.Service
{
    public interface IValuationService
    {
        Task ValuePortfolio(DateTime upToDateTime);
    }

    public class ValuationService : IValuationService
    {
        private readonly IEndOfDayPriceService endOfDayPriceService;
        private readonly ICurrencyRateService currencyRateService;
        private readonly IListingService listingService;

        public ValuationService(IEndOfDayPriceService endOfDayPriceService,
            ICurrencyRateService currencyRateService,
            IListingService listingService)
        {
            this.endOfDayPriceService = endOfDayPriceService;
            this.currencyRateService = currencyRateService;
            this.listingService = listingService;
        }


        public async Task ValuePortfolio(DateTime upToDate)
        {
            //throw new NotImplementedException();
            var listingVolumes = await listingService.GetListingVolumes();
            var firstDay = listingVolumes.Min(lv => lv.Day);
            
            var distinctListings = listingVolumes.Select(lv => lv.Listing).GroupBy(l => l.Id).Select(g => g.First());

            var listingIdsWithDate = distinctListings.SelectMany(l =>
             {
                 var fromDate = listingVolumes
                                 .Where(lv => lv.Listing.Id == l.Id)
                                 .Min(lv => lv.Day);
                 var range = DateTimeExtensions.GetDateRange(fromDate, upToDate);
                 return range.Select(d => new { ListingId = l.Id, Day = d });
             });
            
            var listingIdsWithDateAndQuantity = listingIdsWithDate
                .Select(l =>
                {
                    return listingVolumes
                            .Where(lv => lv.Listing.Id == l.ListingId && lv.Day <= l.Day)
                            .Select(lv => new { l.ListingId, l.Day, lv.Quantity })
                            .OrderByDescending(x => x.Day)
                            .FirstOrDefault();
                })
                .Where(p => p.Quantity > 0);





        }
    }

}

