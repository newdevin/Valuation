using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Valuation.Service.Repository
{
    public interface ISellTradeRepository
    {
        Task<IEnumerable<SellTrade>> GetSellTrades();
    }
}
