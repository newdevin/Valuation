using System.ComponentModel.DataAnnotations.Schema;

namespace Valuation.WorldTradingData.Repository.Entities
{
    [Table("Listing")]
    public class ListingEntity
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Suffix { get; set; }
        [Column("Exchange")]
        public string ExchangeSymbol { get; set; }
        [Column("Currency")]
        public string CurrencySymbol { get; set; }
        public CompanyEntity Company { get; set; }
        public ExchangeEntity Exchange { get; set; }
        public CurrencyEntity Currency { get; set; }
    }

}
