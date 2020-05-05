using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Valuation.Domain;

namespace Valuation.Repository.Entities
{
    [Table("TargetSellPrice")]
    public class TargetSellPriceEntity
    {
        public int Id { get; set; }
        public int ListingId { get; set; }
        public decimal TargetPrice { get; set; }
        public DateTime? PriceReachedOn { get; set; }
        public bool Notified { get; set; }
        public ListingEntity Listing { get; set; }
    }
}
