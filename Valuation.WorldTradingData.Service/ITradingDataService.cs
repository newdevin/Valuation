using LanguageExt;
using System;
using System.Collections.Generic;
using Valuation.Domain;

namespace Valuation.WorldTradingData.Service
{
    public interface ITradingDataService
    {
        public Uri GetEndOfDayPriceUri(DateTime? day, string symbol, string suffix);
        public Uri GetCurrencyRateUri(DateTime? day, string symbol);
        public IEnumerable<EndOfDayPrice> GetPrices(IEnumerable<string> data, int listingId, string currency);
        public IEnumerable<CurrencyRate> GetRates(IEnumerable<string> data, string symbol);
    }
}
