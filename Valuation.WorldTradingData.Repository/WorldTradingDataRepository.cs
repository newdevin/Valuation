using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Valuation.WorldTradingData.Service;

namespace Valuation.WorldTradingData.Repository
{
    public class WorldTradingDataRepository : IWorldTradingDataRepository
    {
        private readonly PicassoDbContext context;

        public WorldTradingDataRepository(PicassoDbContext context)
        {
            this.context = context;
        }
        public string GetToken()
        {
            return context.ApiProviders.First(a => a.Name == "WorldTradingData").Key;
        }
    }
}
