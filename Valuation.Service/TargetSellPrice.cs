using System;
using System.Collections.Generic;
using System.Text;
using Valuation.Domain;

namespace Valuation.Service
{
    public class TargetSellPrice
    {
        public TargetSellPrice(Listing listing, decimal targetPrice)
        {
            Listing = listing;
            TargetPrice = targetPrice;
        }

        public bool IsTargetSellPriceReached(decimal price)
        {

            return price >= TargetPrice;
        }

        public Listing Listing { get; }
        public decimal TargetPrice { get; }
    }
}
