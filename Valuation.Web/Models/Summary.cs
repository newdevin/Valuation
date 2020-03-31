using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Valuation.Web.Models
{
    public class Summary
    {
        public DateTime[] TimeSeries { get; set; }
        public int[] Cost { get; set; }
        public int[] Sold { get; set; }
        public int[] Valuation { get; set; }
        public int[] Profit { get; set; }
        public int[] Realised { get; set; }
    }
}
