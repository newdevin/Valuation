using System;
using Valuation.Domain;

namespace Valuation.Service
{
    public class ListingValuationSummary
    {
        public Listing Listing { get; set; }
        public DateTime Day { get; set; }
        public decimal? CurrentShareValue { get; set; }
        public decimal? PreviousBusinessDayShareValue { get; set; }
        public Currency Currency { get; set; }
        public decimal? Change
        {
            get
            {
                return CurrentShareValue - PreviousBusinessDayShareValue;
            }
        }

        public decimal? PercentChange
        {
            get
            {
                return (CurrentShareValue - PreviousBusinessDayShareValue)/PreviousBusinessDayShareValue * 100;
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as ListingValuationSummary;
            if(other != null)
            {
                return this.Listing.Id == other.Listing.Id && this.Day.Date == other.Day.Date;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Listing.Id.GetHashCode() ^ Day.Date.GetHashCode();
        }
    }
}