using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public interface ITargetPriceReachedService
    {
        Task<IEnumerable<TargetPrice>> GetTargetPrices();
        Task SetNotified(IEnumerable<int> ListingIds);
    }

}
