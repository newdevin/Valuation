
using System;
using System.Collections.Generic;
using System.Text;

namespace Valuation.Domain
{
    public class Exchange
    {
        public Exchange(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(symbol);
            Symbol = symbol;
        }

        public string Symbol { get;  }

     //   public List<Listing> Listings { get; set; }
    }
}
