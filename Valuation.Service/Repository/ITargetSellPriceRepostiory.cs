using System.Collections.Generic;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public interface ITargetSellPriceRepostiory
    {
        Task<IEnumerable<TargetSellPrice>> GetTargetSellPrices();
        Task SetNotified(IEnumerable<int> listingIds);
    }
}