using LanguageExt;
using static LanguageExt.Prelude;

namespace Valuation.Domain
{
    public class Currency
    {
        private Currency(string symbol)
        {
            Symbol = symbol;
        }

        public static Option<Currency> Create(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return None;
            else
                return new Currency(symbol);
        }

        public string Symbol { get; }
    }
}
