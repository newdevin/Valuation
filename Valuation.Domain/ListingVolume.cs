using System;
using System.Collections.Generic;
using System.Text;


namespace Valuation.Domain
{
    public class ListingVolume
    {
        

        public ListingVolume(int id, Listing listing, int quantity, DateTime day)
        {
            Id = id;
            Listing = listing ?? throw new ArgumentException(nameof(listing)); 
            Quantity = quantity;
            Day = day;  
            
        }

        public int Id { get; }
        public Listing Listing { get; }
        public int Quantity { get; }
        public DateTime Day { get; }
    }
}
