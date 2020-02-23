using LanguageExt;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.Service
{
    public interface IListingService
    {
        Task<IEnumerable<Tuple<Listing, Option<DateTime>>>> GetActiveListingWithLastEodPriceDateTime();
        Task<IEnumerable<Listing>> GetActiveListing();
    }

    public class ListingService : IListingService
    {
        public Task<IEnumerable<Listing>> GetActiveListing()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Tuple<Listing, Option<DateTime>>>> GetActiveListingWithLastEodPriceDateTime()
        {
            throw new NotImplementedException();
        }
    }
}
