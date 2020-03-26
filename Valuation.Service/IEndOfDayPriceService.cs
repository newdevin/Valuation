using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.Service
{
    public interface IEndOfDayPriceService
    {
        Task<IEnumerable<EndOfDayPrice>> GetEndOfDayPriceSince( DateTime sinceDay);
    }

    public class EndOfDayPriceService : IEndOfDayPriceService
    {
        private readonly IEndOfDayPriceRepository endOfDayPriceRepository;

        public EndOfDayPriceService(IEndOfDayPriceRepository endOfDayPriceRepository)
        {
            this.endOfDayPriceRepository = endOfDayPriceRepository;
        }
        public Task<IEnumerable<EndOfDayPrice>> GetEndOfDayPriceSince( DateTime sinceDay)
        {
            return endOfDayPriceRepository.GetEndOfDayPriceSince( sinceDay);
        }
    }
}