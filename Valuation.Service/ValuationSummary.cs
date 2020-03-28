using System;

namespace Valuation.Service
{
    public  class ValuationSummary
    {
        public DateTime Day { get; set; }
        public decimal TotalCostInGBP { get; set; }
        public decimal ValuationInGbp { get; set; }
        public decimal TotalRealisedInGBP { get; set; }
        public decimal TotalSellInGBP { get; set; }
        public decimal TotalProfitInGBP { get; set; }
    }
}