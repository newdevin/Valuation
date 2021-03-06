﻿using System;
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

        public Task<IEnumerable<ListingVolume>> GetActiveListingVolumes()
        {
            return listingRepository.GetActiveListingVolumes();
        }

        public Task<IEnumerable<ListingVolume>> GetActiveListingVolumesOnDay(DateTime day)
        {
            return listingRepository.GetActiveListingVolumesOnDay(day);
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
