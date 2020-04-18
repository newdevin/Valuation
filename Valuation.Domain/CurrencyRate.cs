using System;
using System.Collections.Generic;
using System.Text;

namespace Valuation.Domain
{
    public class CurrencyRate
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Rate { get; set; }
        public DateTime Day { get; set; }
    }
}
