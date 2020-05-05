using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuation.Repository.Mapper;
using Valuation.Service;
using Valuation.Service.Repository;

namespace Valuation.Repository
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly PicassoDbContext context;
        private readonly IObjectMapper mapper;

        public ProviderRepository(PicassoDbContext context, IObjectMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public async Task<Provider> GetEmailProvider(string name)
        {
            var entity= await context.Providers.FirstOrDefaultAsync(p => p.ServiceName == name);
            return mapper.MapTo<Provider>(entity);
        }
    }
}
