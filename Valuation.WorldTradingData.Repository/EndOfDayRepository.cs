using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;
using Valuation.WorldTradingData.Service;

namespace Valuation.WorldTradingData.Repository
{
    public class EndOfDayRepository : IEndOfDayRepository
    {
        public Task Save(IEnumerable<EndOfDayPrice> endOfDayPrices)
        {
            throw new NotImplementedException();
        }
    }
}
