using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuation.WorldTradingData.Service;

namespace Valuation.Repository
{
    public class ApiRepository : IApiRepository
    {
        private readonly PicassoDbContext context;

        public ApiRepository(PicassoDbContext context)
        {
            this.context = context;
        }
        public async Task<List<string>> GetTokens(string name)
        {
            return await context.ApiProviders.Where(a => a.Name == name).Select(n => n.ApiKey).ToListAsync();
        }

    }
}
