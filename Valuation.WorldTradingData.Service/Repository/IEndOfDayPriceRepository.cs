using System.Collections.Generic;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.WorldTradingData.Service
{
    public interface IEndOfDayPriceRepository
    {
        Task Save(IEnumerable<EndOfDayPrice> endOfDayPrices);
    }
}