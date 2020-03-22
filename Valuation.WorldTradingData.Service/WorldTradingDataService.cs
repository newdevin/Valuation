using System;
using System.Collections.Generic;
using System.Text;

namespace Valuation.WorldTradingData.Service
{
    public class WorldTradingDataService : IWorldTradingDataService
    {
        private readonly Uri baseUri;
        private readonly IWorldTradingDataRepository worldTradingDataRepository;
        private string token;

        public WorldTradingDataService(Uri baseUri, IWorldTradingDataRepository worldTradingDataRepository)
        {
            this.baseUri = baseUri;
            this.worldTradingDataRepository = worldTradingDataRepository;
            token = worldTradingDataRepository.GetToken();
        }

        public Uri GetEndOfDayPriceUri(DateTime? dateTime, string symbol)
        {
            
            if (dateTime.HasValue)
                return new Uri($"{baseUri}api.v1/history?output=csv&api_token={token}&symbol={symbol}&date_from={dateTime.Value.ToString("yyyy-MM-dd")}");
            else
                return new Uri($"{baseUri}api.v1/history?output=csv&api_token={token}&symbol={symbol}");
            

        }
    }
}
