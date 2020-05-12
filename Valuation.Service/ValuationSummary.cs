using System;

namespace Valuation.Service
{
    public  class ValuationSummary
    {
        public int Id { get; set; }
        public DateTime Day { get; set; }
        public decimal TotalCostInGbp { get; set; }
        public decimal ValuationInGbp { get; set; }
        public decimal TotalRealisedInGbp { get; set; }
        public decimal TotalSellInGbp { get; set; }
        public decimal TotalProfitInGbp { get; set; }
        public decimal TotalCashInvestedInGbp { get; set; }
        public decimal TotalCashWithdrawnInGbp { get; set; }

    }
}