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
    }
}