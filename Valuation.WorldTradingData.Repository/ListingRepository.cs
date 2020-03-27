
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;
using Valuation.Infrastructure;
using Valuation.Service.Repository;
using Valuation.Repository.Entities;

namespace Valuation.Repository
{
    public class ListingRepository : IListingRepository
    {
        private readonly PicassoDbContext context;
        private readonly IObjectMapper mapper;

        public ListingRepository(PicassoDbContext context, IObjectMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public async Task<IEnumerable<Tuple<Listing, DateTime?>>> GetActiveListingWithLastPriceFetchDay()
        {

            var allListingEntities = context.ListingVolumes
                .Include(lv => lv.Listing.Currency);

            var activeListingVolumeEntities = await context.ListingVolumes
                                    .Where(lv => lv.Day == allListingEntities.Where(al => al.Listing.Id == lv.Listing.Id).Max(al => al.Day))
                                    .Include(lv => lv.Listing.Currency)
                                    .Select(lv => lv).ToListAsync();

            var eodPrices = await context.EndOfDayPrices
                                    .Where(eodPrice => eodPrice.Day == context.EndOfDayPrices.Where(al => al.Listing.Id == eodPrice.Listing.Id).Max(al => al.Day))
                                    .Include(eod => eod.Listing.Currency)
                                    .ToListAsync();

            var p = activeListingVolumeEntities.GroupJoin(eodPrices, lv => lv.Listing.Id, eod => eod.Listing.Id, (lv, eod) => new { ListingVolume = lv, EndOfDayPrice = eod })
                .SelectMany(x => x.EndOfDayPrice.DefaultIfEmpty(), (x, y) => new { ListingVolume = x.ListingVolume, eodPrice = x.EndOfDayPrice })
                .ToList();

            var listingsWithDate = p
                .Where(x => x.ListingVolume.Quantity > 0)
                .Select(x => Tuple.Create(mapper.MapTo<ListingEntity, Listing>(x.ListingVolume.Listing), x.eodPrice?.FirstOrDefault()?.Day))
                .ToList();

            return listingsWithDate;
        }

        public async Task<IEnumerable<ListingVolume>> GetListingVolumes()
        {
            var entities = await context.ListingVolumes.Include(lv=>lv.Listing.Currency).ToListAsync();
            return entities.Select(e => mapper.MapTo<ListingVolumeEntity, ListingVolume>(e));
        }
    }
}
