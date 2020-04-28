using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.Service
{
    public interface IValuationService
    {
        Task ValuePortfolio();
        Task ValuePortfolio(DateTime upToDateTime);
        Task<IEnumerable<ValuationSummary>> GetValuationSummary();
        Task<IEnumerable<ListingValuation>> GetValuations(DateTime date);

    }

}

