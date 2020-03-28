using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Valuation.Service;
using Valuation.Service.Repository;

namespace Valuation.Repository
{
    public class SellTradeRepository : ISellTradeRepository
    {
        private readonly PicassoDbContext context;

        public SellTradeRepository(PicassoDbContext context)
        {
            this.context = context;
        }
        public async Task<IEnumerable<SellTrade>> GetSellTrades()
        {
            return await context.SellTrades.Include(st => st.BuyTrade).ToListAsync();
        }
    }

}
