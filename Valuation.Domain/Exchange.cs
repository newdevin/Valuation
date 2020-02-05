using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Text;

namespace Valuation.Domain
{
    public class Exchange
    {
        private Exchange(string symbol)
        {
            Symbol = symbol;
        }

        public static Option<Exchange> Create(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return None;
            else
                return new Exchange(symbol);
        }


        public string Symbol { get; }
    }
}
