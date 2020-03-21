using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Valuation.WorldTradingData.Repository.Entities
{
    [Table("EndOfDayPrice")]
    public class EndOfDayPriceEntity
    {
        public int Id { get; set; }
        public ListingEntity Listing { get; set; }
        public DateTime Day { get; set; }
        public decimal? Open { get; set; }
        public decimal? Close { get; set; }
        public decimal? High { get; set; }
        public decimal? Loow { get; set; }
        public int? Volume { get; set; }
    }
}
