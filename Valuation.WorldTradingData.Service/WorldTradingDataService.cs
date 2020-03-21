using LanguageExt;
using System;
using System.Collections.Generic;
using System.Text;

namespace Valuation.WorldTradingData.Service
{

    public class WorldTradingDataService : IWorldTradingDataService
    {
        private readonly Uri baseUri;
        private readonly string token;

        public WorldTradingDataService(Uri baseUri, string token)
        {
            this.baseUri = baseUri;
            this.token = token;
        }

        public Uri GetEndOfDayPriceUri(Option<DateTime> dateTime, string symbol)
        {
            return dateTime.Match(dt =>
            new Uri($"{baseUri}api.v1/history?output=csv&api_token={token}&symbol={symbol}&date_from={dt.ToString("yyyy-MM-dd")}"),
             () => new Uri($"{baseUri}api.v1/history?output=csv&api_token={token}&symbol={symbol}"));

        }
    }
}
