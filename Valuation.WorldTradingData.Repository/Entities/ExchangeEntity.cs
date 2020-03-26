using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Valuation.Repository.Entities
{
    [Table("Exchange")]
    public class ExchangeEntity
    {
        [Key]
        public string Symbol { get; set; }
    }

}
