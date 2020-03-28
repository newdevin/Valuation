using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuation.Service;
using Valuation.Service.Repository;

namespace Valuation.Repository
{
    public class BuyTradeRepository : IBuyTradeRepository
    {
        private readonly PicassoDbContext context;

        public BuyTradeRepository(PicassoDbContext context)
        {
            this.context = context;
        }
        public async Task<IEnumerable<BuyTrade>> GetBuyTrades()
        {
            return await context.BuyTrades.ToListAsync();
        }
    }

}
