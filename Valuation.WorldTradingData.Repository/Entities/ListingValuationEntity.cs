using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Valuation.Repository.Entities
{
    [Table("Valuation")]
    public class ListingValuationEntity
    {
        public int Id { get; set; }
        public DateTime Day { get; set; }
        public int ListingId { get; set; }
        public decimal SharePrice { get; set; }
        public string ShareCurrency { get; set; }
        public int Quantity { get; set; }
        [Column("GBPCurrencyRate")]
        public decimal Rate { get; set; }
        public decimal TotalValue { get; set; }
        public decimal TotalValueInGbp { get; set; }
        
    }
}
