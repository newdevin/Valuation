using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.Service.Repository
{
    public interface IValuationRepository
    {
        Task Save(IEnumerable<ListingValuation> valuations, IEnumerable<ValuationSummary> summary);
        Task<IEnumerable<ValuationSummary>> GetValuationSummary();
    }
}
