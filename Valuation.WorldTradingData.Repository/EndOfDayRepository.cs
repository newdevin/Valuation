using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;
using Valuation.Infrastructure;
using Valuation.WorldTradingData.Repository.Entities;
using Valuation.WorldTradingData.Service;

namespace Valuation.WorldTradingData.Repository
{
    public class EndOfDayRepository : IEndOfDayRepository
    {
        private readonly PicassoDbContext context;
        private readonly IObjectMapper mapper;

        public EndOfDayRepository(PicassoDbContext context, IObjectMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public async Task Save(IEnumerable<EndOfDayPrice> endOfDayPrices)
        {

            foreach (var eodPrice in endOfDayPrices)
            {
                var e = context.EndOfDayPrices.Where(eod => eod.Listing.Id == eodPrice.ListingId && eod.Day == eodPrice.Day).ToList();
                context.EndOfDayPrices.RemoveRange(e);
            }
            await context.SaveChangesAsync();
            var entities = endOfDayPrices
                .Select(endOfDayPrice => mapper.MapTo<EndOfDayPrice, EndOfDayPriceEntity>(endOfDayPrice))
                .ToList();
            context.EndOfDayPrices.AddRange(entities);
            await context.SaveChangesAsync();
        }
    }
}
