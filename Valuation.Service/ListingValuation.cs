using System;
using System.Collections.Generic;
using System.Text;

namespace Valuation.Domain
{
    public class ListingValuation
    {
        public int Id { get; set; }
        public DateTime Day { get; set; }
        public int ListingId { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal TotalValue { get; set; }
        public decimal TotalValueInGbp { get; set; }


    }
}
