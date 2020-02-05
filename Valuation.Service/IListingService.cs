using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.Service
{
    public interface IListingService
    {
        IAsyncEnumerable<Listing> GetActiveListing();
    }

    public class ListingService : IListingService
    {
        public IAsyncEnumerable<Listing> GetActiveListing()
        {
            throw new NotImplementedException();
        }
    }
}
