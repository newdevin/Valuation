using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Valuation.Repository.Entities
{
    [Table("EndOfDayPriceLog")]
    public class EndOfDayPriceLogEntity
    {
        public int Id { get; set; }
        public DateTime Day { get; set; }
        public DateTime StartedOn { get; set; }
        public DateTime? FinishedOn { get; set; }
        public bool HasErrored { get; set; }
    }
}
