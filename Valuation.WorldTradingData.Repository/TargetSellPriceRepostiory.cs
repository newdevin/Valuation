using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;
using Valuation.Repository.Mapper;
using Valuation.Service;
using Microsoft.EntityFrameworkCore;

namespace Valuation.Repository
{
    public class TargetSellPriceRepostiory : ITargetSellPriceRepostiory
    {
        private readonly PicassoDbContext context;
        private readonly IObjectMapper mapper;

        public TargetSellPriceRepostiory(PicassoDbContext context, IObjectMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public async Task<IEnumerable<TargetSellPrice>> GetTargetSellPrices()
        {
            var entities = await context.TargetSellPrices
                .Include(t=> t.Listing)
                .Where(t => t.PriceReachedOn == null)
                .ToListAsync();
            return entities.Select(mapper.MapTo<TargetSellPrice>);
        }

        public async Task SetNotified(IEnumerable<int> listingIds)
        {
            var entities = await context.TargetSellPrices.Where(t => t.PriceReachedOn == null).ToListAsync();
            var toBeUpdated = entities.Where(e => listingIds.Any(lid => lid == e.ListingId)).ToList();
            foreach (var entity in toBeUpdated)
            {
                entity.PriceReachedOn = DateTime.Now.Date;
                entity.Notified = true;
            }
            await context.SaveChangesAsync();
        }
    }
}
