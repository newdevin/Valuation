using System;

namespace Valuation.Service
{
    public class SellTrade
    {
        public int Id { get; set; }
        public int BuyTradeId { get; set; }
        public int ListingId { get; set; }
        public decimal PricePerShare { get; set; }
        public string PricePerShareCurrency { get; set; }
        public int Quantity { get; set; }
        public DateTime SoldOn { get; set; }
        public decimal TotalReceivedInGbp { get; set; }
        public BuyTrade BuyTrade { get; set; }

    }
}