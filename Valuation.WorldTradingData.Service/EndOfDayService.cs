using System;
using System.Threading.Tasks;
using Valuation.Service;

namespace Valuation.WorldTradingData.Service
{
    public class EndOfDayService : IEndOfDayService
    {
        private readonly IEndOfDayRepository endOfDayRepository;
        private readonly IListingService listingService;

        public EndOfDayService(IEndOfDayRepository endOfDayRepository, IListingService listingService)
        {
            this.endOfDayRepository = endOfDayRepository;
            this.listingService = listingService;
        }
        public Task DownloadEndOfDayPrices()
        {
            throw new NotImplementedException();
        }
    }
}
