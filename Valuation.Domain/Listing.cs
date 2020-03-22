
using System;
using System.Collections.Generic;
using System.Text;

namespace Valuation.Domain
{
    public class Listing
    {
       
        public Listing(int id, Company company, Exchange exchange, Currency currency, string symbol, string suffix)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(symbol);
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

    }
}
