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
        Task<IEnumerable<Tuple<Listing, Option<DateTime>>>> GetActiveListingWithLastEodPriceDateTime();
        Task<IEnumerable<Listing>> GetActiveListing();
    }

    public class ListingService : IListingService
    {
        private readonly IListingRepository listingRepository;

        public ListingService(IListingRepository listingRepository)
        {
            this.listingRepository = listingRepository;
        }
        public Task<IEnumerable<Listing>> GetActiveListing()
        {
            return listingRepository.GetActiveListing();
        }

        public Task<IEnumerable<Tuple<Listing, Option<DateTime>>>> GetActiveListingWithLastEodPriceDateTime()
        {
            throw new NotImplementedException();
        }
    }
}
