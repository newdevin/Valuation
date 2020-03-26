using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Valuation.Repository.Entities
{
    [Table("ListingVolume")]
    public class ListingVolumeEntity
    {
        public int Id { get; set; }
        public ListingEntity Listing { get; set; }
        public int Quantity { get; set; }
        public DateTime Day { get; set; }
    }
}
