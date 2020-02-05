using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Text;

namespace Valuation.Domain
{
    public class Listing
    {
       
        private Listing(int id, Company company, Exchange exchange, Currency currency, string symbol, string suffix)
        {
            Id = id;
            Company = company;
            Exchange = exchange;
            Currency = currency;
            Symbol = symbol;
            Suffix = suffix;
        }

        public int Id { get; }
        public Company Company { get; }
        public Exchange Exchange { get; }
        public Currency Currency { get; }
        public string Symbol { get; }
        public string Suffix { get; }

        public static Option<Listing> Create(int id, Company company, Exchange exchange, Currency currency, string symbol, string suffix)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return None;
            else
                return new Listing(id, company, exchange, currency, symbol, suffix);
        }
    }
}
