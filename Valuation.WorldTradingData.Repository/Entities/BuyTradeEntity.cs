using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Valuation.Repository.Entities
{
    [Table("BuyTrade")]
    public class BuyTradeEntity
    {
        public int Id { get; set; }
        public int ListingId { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerShare { get; set; }
        public string PricePerShareCurrency { get; set; }
        public DateTime BoughtOn { get; set; }
        public decimal TotalPaidInGbp { get; set; }


    }
}
