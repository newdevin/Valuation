using System.Collections.Generic;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public interface ITargetPriceRepostiory
    {
        Task<IEnumerable<TargetPrice>> GetTargetPrices();
        Task SetNotified(IEnumerable<int> listingIds);
    }
}