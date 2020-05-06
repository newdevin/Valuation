using LanguageExt;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public class TargetSellPriceReachedService : ITargetSellPriceReachedService
    {
        private readonly ITargetSellPriceRepostiory targetSellPriceRepostiory;
        private readonly IListingService listingService;

        public TargetSellPriceReachedService(ITargetSellPriceRepostiory targetSellPriceRepostiory, IListingService listingService)
        {
            this.targetSellPriceRepostiory = targetSellPriceRepostiory;
            this.listingService = listingService;
        }
        public async Task<IEnumerable<TargetSellPrice>> GetTargetSellPrices()
        {
            var activeListingVolumes = await listingService.GetActiveListingVolumes();
            var activeListingIds = activeListingVolumes.Select(lv => lv.Listing.Id).Distinct();
            var targetSellPrices = await targetSellPriceRepostiory.GetTargetSellPrices();
            return targetSellPrices.Where(t => activeListingIds.Any(lid => lid == t.Listing.Id)).ToList();
        }

        public Task SetNotified(IEnumerable<int> listingIds)
        {
            return targetSellPriceRepostiory.SetNotified(listingIds);
        }
    }

}
