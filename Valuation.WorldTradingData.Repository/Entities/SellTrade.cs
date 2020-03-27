using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Valuation.Repository.Entities
{
    [Table("SellTrade")]
    public class SellTradeEntity
    {
        public int Id { get; set; }
        public int BuyTradeId { get; set; }
        public int ListingId { get; set; }
        public decimal PricePerShare { get; set; }
        public string PricePerShareCurrency { get; set; }
        public int Quantity { get; set; }
        public DateTime SoldOn { get; set; }
        public decimal TotalReceivedInGbp { get; set; }

        public BuyTradeEntity BuyTrade { get; set; }
    }
}
