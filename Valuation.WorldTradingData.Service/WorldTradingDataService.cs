﻿using System;
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

        public Uri GetEndOfDayPriceUri(DateTime? dateTime, string symbol, string suffix)
        {
            var uriString = $"{baseUri}api/v1/history?output=csv&api_token={token}";
            if (suffix != null)
                uriString += $"&symbol={symbol}.{suffix}";
            else
                uriString += $"&symbol={symbol}";
            if (dateTime.HasValue)
                uriString += $"&date_from={dateTime.Value.ToString("yyyy-MM-dd")}";

            return new Uri(uriString);                    


        }
    }
}
