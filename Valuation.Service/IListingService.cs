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
        Task<IEnumerable<Tuple<Listing, DateTime?>>> GetActiveListingWithLastEodPriceDateTime();
        Task<IEnumerable<ListingVolume>> GetListingVolumes();

        Task<IEnumerable<ListingVolume>> GetActiveListingVolumes();
    }
}
