namespace Valuation.Service
{
    public class PortfolioValuation
    {
        public ValuationSummary ValuationSummary { get; set; }
        public decimal TotalValuationChange { get; set; }
        public decimal  TotalCostChange{ get; set; }
        public decimal TotalProfitChange { get; set; }
        public decimal TotalRealisedChange { get; set; }
        public decimal TotalSellChanged { get; set; }

    }
}