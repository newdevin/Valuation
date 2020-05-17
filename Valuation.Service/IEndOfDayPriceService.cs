using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.Service
{
    public interface IEndOfDayPriceService
    {
        Task<IEnumerable<EndOfDayPrice>> GetEndOfDayPriceSince(DateTime sinceDay);

        Task<IEnumerable<EndOfDayPrice>> GetLatestEndOfDayPrices(IEnumerable<int> listingIds);
        Task<IEnumerable<EndOfDayPrice>> GetPrices(DateTime day, IEnumerable<int> listingIds);
    }
}