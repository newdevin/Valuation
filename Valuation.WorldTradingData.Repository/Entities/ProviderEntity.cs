using System.ComponentModel.DataAnnotations.Schema;

namespace Valuation.Repository.Entities
{
    [Table("Provider")]
    public class ProviderEntity
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string ServiceAgent { get; set; }
        public string Key { get; set; }
    }
}