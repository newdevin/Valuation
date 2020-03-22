
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;
using Valuation.Infrastructure;
using Valuation.Service.Repository;
using Valuation.WorldTradingData.Repository.Entities;

namespace Valuation.WorldTradingData.Repository
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
        public async Task<IEnumerable<Listing>> GetActiveListing()
        {

            var allListingEntities = context.ListingVolumes
                .Include(lv=> lv.Listing);

            var activeListingsEntities = await context.ListingVolumes
                                    .Where(lv => lv.Day == allListingEntities.Where(al => al.Listing.Id == lv.Listing.Id).Max(al => al.Day) && lv.Quantity > 0)
                                    .Select(lv => lv.Listing)
                                    .ToListAsync() ;
            var activeListings = activeListingsEntities.Select (activeListingsEntity =>  mapper.MapTo<ListingEntity, Listing>(activeListingsEntity)).ToList();
            return activeListings;


        }
    }
}
