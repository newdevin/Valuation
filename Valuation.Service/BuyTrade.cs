using System;

namespace Valuation.Service
{
    public class BuyTrade
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