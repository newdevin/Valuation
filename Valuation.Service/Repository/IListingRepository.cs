using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.Service.Repository
{
    public interface IListingRepository
    {
        Task<IEnumerable<Tuple<Listing, DateTime?>>> GetActiveListingWithLastPriceFetchDay();
        Task<IEnumerable<ListingVolume>> GetListingVolumes();
        Task<IEnumerable<ListingVolume>> GetActiveListingVolumes();
        Task<IEnumerable<ListingVolume>> GetActiveListingVolumesOnDay(DateTime day);
    }
}
