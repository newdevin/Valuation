using LanguageExt;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public class TargetPriceReachedService : ITargetPriceReachedService
    {
        private readonly ITargetPriceRepostiory targetPriceRepostiory;
        private readonly IListingService listingService;

        public TargetPriceReachedService(ITargetPriceRepostiory targetPriceRepostiory, IListingService listingService)
        {
            this.targetPriceRepostiory = targetPriceRepostiory;
            this.listingService = listingService;
        }
        public async Task<IEnumerable<TargetPrice>> GetTargetPrices()
        {
            var activeListingVolumes = await listingService.GetActiveListingVolumes();
            var activeListingIds = activeListingVolumes.Select(lv => lv.Listing.Id).Distinct();
            var targetPrices = await targetPriceRepostiory.GetTargetPrices();
            return targetPrices.Where(t =>
            {
                return
                string.Equals("Sell", t.TargetType, System.StringComparison.OrdinalIgnoreCase) && activeListingIds.Any(lid => lid == t.Listing.Id)
                ||
                string.Equals("Buy", t.TargetType, System.StringComparison.OrdinalIgnoreCase);
            }
            ).ToList();
        }

        public Task SetNotified(IEnumerable<int> listingIds)
        {
            return targetPriceRepostiory.SetNotified(listingIds);
        }
    }

}
