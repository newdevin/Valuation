﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Valuation.Repository.Entities
{
    [Table("Currency")]
    public class CurrencyEntity
    {
        [Key]
        public string Symbol { get; set; }
    }

}
