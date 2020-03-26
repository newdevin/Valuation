using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Valuation.Domain;
using Valuation.Service.Repository;

namespace Valuation.Service
{
    public class ListingService : IListingService
    {
        private readonly IListingRepository listingRepository;

        public ListingService(IListingRepository listingRepository)
        {
            this.listingRepository = listingRepository;
        }
        public Task<IEnumerable<Tuple<Listing, DateTime?>>> GetActiveListingWithLastEodPriceDateTime()
        {
            return listingRepository.GetActiveListingWithLastPriceFetchDay();
        }

        public Task<IEnumerable<ListingVolume>> GetListingVolumes()
        {
            return listingRepository.GetListingVolumes();
        }
    }
}
