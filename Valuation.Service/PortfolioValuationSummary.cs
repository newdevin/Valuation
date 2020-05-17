using System.Collections.Generic;

namespace Valuation.Service
{
    public class PortfolioValuationSummary
    {
        public ValuationSummary ValuationSummary { get; set; }
        public IEnumerable<ListingValuationSummary> ListingValuationSummaries { get; set; }
        public decimal TotalValuationChange { get; set; }
        public decimal TotalCostChange { get; set; }
        public decimal TotalProfitChange { get; set; }
        public decimal TotalRealisedChange { get; set; }
        public decimal TotalSellChanged { get; set; }
        public decimal TotalCashInvestedChange { get; set; }
        public decimal TotalCashWithdrawnChange { get; set; }

    }
}