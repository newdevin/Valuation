using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Valuation.Service.Repository
{
    public interface IBuyTradeRepository
    {
        Task<IEnumerable<BuyTrade>> GetBuyTrades();
    }
}
