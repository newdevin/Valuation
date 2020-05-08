using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Valuation.Domain;

namespace Valuation.Repository.Entities
{
    [Table("TargetPrice")]
    public class TargetPriceEntity
    {
        public int Id { get; set; }
        public int ListingId { get; set; }
        public decimal TargetPrice { get; set; }
        public string TargetType { get; set; }
        public DateTime? PriceReachedOn { get; set; }
        public bool Notified { get; set; }
        public ListingEntity Listing { get; set; }
    }
}
