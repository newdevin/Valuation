using System;
using System.Collections.Generic;
using System.Text;


namespace Valuation.Domain
{
    public class ListingVolume
    {
        private readonly int id;
        private readonly Listing listing;
        private readonly int quantity;
        private readonly DateTime day;

        public ListingVolume(int id, Listing listing, int quantity, DateTime day)
        {
            this.listing = listing ?? throw new ArgumentException(nameof(listing));
            this.id = id;
            this.quantity = quantity;
            this.day = day;
        }

    }
}
