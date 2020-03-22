using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Valuation.WorldTradingData.Repository.Entities
{
    [Table("APIProvider")]
    public class ApiProdiverEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
    }
}
