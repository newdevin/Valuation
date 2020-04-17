using System;
using System.Collections.Generic;
using System.Text;

namespace Valuation.Domain
{
    public class CurrencyRate
    {
        public CurrencyRate(int id, string from, string to , decimal rate , DateTime day)
        {
            Id = id;
            From = from;
            To = to;
            Rate = rate;
            Day = day;
        }
        public int Id { get;}
        public string From { get;}
        public string To { get;}
        public decimal Rate { get;}
        public DateTime Day { get;}
    }
}
