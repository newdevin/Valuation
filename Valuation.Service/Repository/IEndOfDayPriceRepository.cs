using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.Service
{
    public interface IEndOfDayPriceRepository
    {
        Task Save(IEnumerable<EndOfDayPrice> endOfDayPrices);
        Task<IEnumerable<EndOfDayPrice>> GetEndOfDayPriceSince( DateTime sinceDay);
    }
}