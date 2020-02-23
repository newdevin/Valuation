using LanguageExt;
using System;

namespace Valuation.WorldTradingData.Service
{
    public interface IWorldTradingDataService
    {
        public Uri GetEndOfDayPriceUri(Option<DateTime> dateTime, string symbol);
    }
}
