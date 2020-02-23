using System;
using System.Collections.Generic;
using System.Text;

namespace Valuation.Domain
{
    public class EndOfDayPrice
    {
        public EndOfDayPrice(int listingId, DateTime day, decimal openPrice, decimal closePrice,
            decimal highPrice, decimal lowPrice, int volume)
        {
            ListingId = listingId;
            Day = day;
            OpenPrice = openPrice;
            ClosePrice = closePrice;
            HighPrice = highPrice;
            LowPrice = lowPrice;
            Volume = volume;
        }

        public int ListingId { get; }
        public DateTime Day { get; }
        public decimal OpenPrice { get; }
        public decimal ClosePrice { get; }
        public decimal HighPrice { get; }
        public decimal LowPrice { get; }
        public int Volume { get; }

        public static EndOfDayPrice Create(int listingId, DateTime day, decimal openPrice, decimal closePrice, decimal highPrice, decimal lowPrice, int volume)
        {
            return new EndOfDayPrice(listingId, day, openPrice, closePrice, highPrice, lowPrice, volume);
        }
    }
}
