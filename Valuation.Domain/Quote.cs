using System;
using System.Collections.Generic;
using System.Text;

namespace Valuation.Domain
{
    public class Quote
    {
        public Quote(Listing listing, decimal? price, DateTime? day)
        {
            Listing = listing;
            Price = price;
            Day = day;
        }

        public Listing Listing { get; }
        public decimal? Price { get; }
        public DateTime? Day { get; }

        public bool IsEmpty { get { return Listing is null; } }

        public static Quote Empty()
        {
            return new Quote(null, null, null);
        }
    }
}
