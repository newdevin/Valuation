using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;
using Valuation.Infrastructure;
using Valuation.Repository.Entities;
using Valuation.Service;
using Valuation.WorldTradingData.Service;

namespace Valuation.Repository
{
    public class EndOfDayPriceRepository : IEndOfDayPriceRepository
    {
        private readonly PicassoDbContext context;
        private readonly IObjectMapper mapper;

        public EndOfDayPriceRepository(PicassoDbContext context, IObjectMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<EndOfDayPrice>> GetEndOfDayPriceSince(DateTime sinceDay)
        {
            var entities = await context.EndOfDayPrices.Include(e=> e.Listing)
                .Where(eod => eod.Day >= sinceDay)
                .ToListAsync();
            return entities.Select(e => mapper.MapTo<EndOfDayPriceEntity, EndOfDayPrice>(e));
        }

        public async Task Save(IEnumerable<EndOfDayPrice> endOfDayPrices)
        {

            foreach (var eodPrice in endOfDayPrices)
            {
                var e = context.EndOfDayPrices.Where(eod => eod.Listing.Id == eodPrice.ListingId && eod.Day == eodPrice.Day).ToList();
                context.EndOfDayPrices.RemoveRange(e);
            }

            var entities = endOfDayPrices
                .Select(endOfDayPrice => mapper.MapTo<EndOfDayPrice, EndOfDayPriceEntity>(endOfDayPrice))
                .ToList();
            context.EndOfDayPrices.AddRange(entities);
            await context.SaveChangesAsync();
        }
    }
}
