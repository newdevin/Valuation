using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public interface ITargetSellPriceReachedService
    {
        Task<IEnumerable<TargetSellPrice>> GetTargetSellPrices();
        Task SetNotified(IEnumerable<int> ListingIds);
    }

}
