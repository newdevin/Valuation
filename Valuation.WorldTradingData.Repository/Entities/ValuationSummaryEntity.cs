using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Valuation.Repository.Entities
{
    [Table("ValuationSummary")]
    public class ValuationSummaryEntity
    {
        public int Id { get; set; }
        public DateTime Day { get; set; }
        public decimal ValuationInGbp { get; set; }
        public decimal TotalCostInGBP { get; set; }
        public decimal TotalSellInGBP { get; set; }
        public decimal TotalProfitInGBP { get; set; }
        public decimal TotalRealisedInGBP { get; set; }

    }
}
