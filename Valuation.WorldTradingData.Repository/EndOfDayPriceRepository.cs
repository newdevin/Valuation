using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

using System.Threading.Tasks;
using Valuation.Domain;
using Valuation.Repository.Entities;
using Valuation.Repository.Mapper;
using Valuation.Service;


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

        public async Task<IEnumerable<EndOfDayPrice>> GetLatestEndOfDayPrices(IEnumerable<int> listingIds)
        {
            var entities = new List<EndOfDayPriceEntity>();

            foreach(var listingId in listingIds)
            {
                var entity = await context.EndOfDayPrices
                    .Include(e=> e.Listing)
                    .Where(e => e.ListingId == listingId).OrderByDescending(e => e.Day)
                    .FirstOrDefaultAsync() ;
                entities.Add(entity);
            }

            return entities.Select(mapper.MapTo<EndOfDayPrice>);
        }

        public async Task<IEnumerable<EndOfDayPrice>> GetEndOfDayPriceSince(DateTime sinceDay)
        {
            var entities = await context.EndOfDayPrices.Include(e=> e.Listing)
                .Where(eod => eod.Day >= sinceDay)
                .ToListAsync();
            return entities.Select(mapper.MapTo<EndOfDayPrice>);
        }

        public async Task Save(IEnumerable<EndOfDayPrice> endOfDayPrices)
        {

            foreach (var eodPrice in endOfDayPrices)
            {
                var e = context.EndOfDayPrices.Where(eod => eod.Listing.Id == eodPrice.ListingId && eod.Day == eodPrice.Day).ToList();
                context.EndOfDayPrices.RemoveRange(e);
            }

            var entities = endOfDayPrices
                .Select(endOfDayPrice => mapper.MapTo<EndOfDayPriceEntity>(endOfDayPrice))
                .ToList();
            context.EndOfDayPrices.AddRange(entities);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EndOfDayPrice>> GetEndOfDayPriceOnDay(DateTime day)
        {
            var entities = await context.EndOfDayPrices
                .Include(eod => eod.Listing.Currency)
                .Where(eod => eod.Day.Date == day.Date)
                .ToListAsync();
            return entities.Select(mapper.MapTo<EndOfDayPrice>);
        }
    }
}
