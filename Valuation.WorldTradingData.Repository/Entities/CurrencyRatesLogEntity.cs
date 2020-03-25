using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Valuation.WorldTradingData.Repository.Entities
{
    [Table("CurrencyRatesLog")]
    public class CurrencyRatesLogEntity
    {
        public int Id { get; set; }
        public DateTime Day { get; set; }
        public DateTime StartedOn { get; set; }
        public DateTime? FinishedOn { get; set; }
        public bool HasErrored { get; set; }
    }
}
