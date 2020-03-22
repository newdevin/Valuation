using LanguageExt;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;
using Valuation.Service.Repository;

namespace Valuation.Service
{
    public interface IListingService
    {
        Task<IEnumerable<Tuple<Listing, DateTime?>>> GetActiveListingWithLastEodPriceDateTime();
    }

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

    }
}
