using LanguageExt;
using System;

namespace Valuation.WorldTradingData.Service
{
    public interface IWorldTradingDataService
    {
        public Uri GetEndOfDayPriceUri(DateTime? dateTime, string symbol, string suffix);
    }
}
