using System;
using System.Collections.Generic;

namespace Valuation.Domain
{
    public class Currency
    {
        public Currency(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(symbol);
            Symbol = symbol;
        }

        public string Symbol { get; }
      //  public List<Listing> Listings { get; set; }
    }
}
