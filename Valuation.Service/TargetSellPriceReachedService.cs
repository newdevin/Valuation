using System.Collections.Generic;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public class TargetSellPriceReachedService : ITargetSellPriceReachedService
    {
        private readonly ITargetSellPriceRepostiory targetSellPriceRepostiory;

        public TargetSellPriceReachedService(ITargetSellPriceRepostiory targetSellPriceRepostiory)
        {
            this.targetSellPriceRepostiory = targetSellPriceRepostiory;
        }
        public Task<IEnumerable<TargetSellPrice>> GetTargetSellPrices()
        {
            return targetSellPriceRepostiory.GetTargetSellPrices();
        }

        public Task SetNotified(IEnumerable<int> listingIds)
        {
            return targetSellPriceRepostiory.SetNotified(listingIds);
        }
    }

}
