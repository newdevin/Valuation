using LanguageExt;
using System;

namespace Valuation.WorldTradingData.Service
{
    public interface IWorldTradingDataService
    {
        public Uri GetEndOfDayPriceUri(DateTime? day, string symbol, string suffix);
        public Uri GetCurrencyRateUri(DateTime? day, string symbol);
    }
}
