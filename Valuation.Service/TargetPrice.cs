using System;
using System.Collections.Generic;
using System.Text;
using Valuation.Domain;

namespace Valuation.Service
{
    public class TargetPrice
    {
        public TargetPrice(Listing listing, decimal targetPrice, string targetType)
        {
            Listing = listing;
            Target = targetPrice;
            TargetType = targetType;
        }

        public bool IsTargetPriceReached(decimal price)
        {
            if (string.Equals("Sell", TargetType, StringComparison.OrdinalIgnoreCase))
                return price >= Target;
            else if (string.Equals("Buy", TargetType, StringComparison.OrdinalIgnoreCase))
                return price <= Target;
            else
                throw new InvalidOperationException($"TargetType: {TargetType} is not supported. Only TargetType of 'Buy' and 'Sell' are supported");
        }

        public Listing Listing { get; }
        public decimal Target { get; }
        public string TargetType { get; }
    }
}
