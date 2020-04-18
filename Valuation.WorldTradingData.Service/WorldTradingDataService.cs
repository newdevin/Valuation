using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.WorldTradingData.Service
{
    public class WorldTradingDataService : ITradingDataService
    {
        private readonly Uri baseUri;
        private readonly IApiRepository worldTradingDataRepository;
        private string token;

        public WorldTradingDataService( Uri baseUri, IApiRepository worldTradingDataRepository)
        {
        
            this.baseUri = baseUri;
            this.worldTradingDataRepository = worldTradingDataRepository;
            GetToken();
        }

        public void GetToken()
        {
            if(token is null)
            {
                var task = worldTradingDataRepository.GetTokens("WorldTradingData");
                token = task.Result.First();
            }
        }

        public Uri GetCurrencyRateUri(DateTime? day, string symbol)
        {
            
            var uriString = $@"{baseUri}/api/v1/forex_history?api_token={token}&base={symbol}&convert_to=GBP&output=csv";
            if (day.HasValue)
                uriString += $"&date_from={day.Value:yyyy-MM-dd}";

            return new Uri(uriString);

        }

        public Uri GetEndOfDayPriceUri(DateTime? dateTime, string symbol, string suffix)
        {
            var uriString = $"{baseUri}api/v1/history?output=csv&api_token={token}";
            if (suffix != null)
                uriString += $"&symbol={symbol}.{suffix}";
            else
                uriString += $"&symbol={symbol}";
            if (dateTime.HasValue)
                uriString += $"&date_from={dateTime.Value:yyyy-MM-dd}";

            return new Uri(uriString);

        }

        public IEnumerable<EndOfDayPrice> GetPrices(IEnumerable<string> data, int listingId, string currency)
        {
            //Date,Open,Close,High,Low,Volume
            return data
                    .Where(d => !string.IsNullOrWhiteSpace(d))
                    .Select(d =>
                    {
                        var p = d.Split(',', StringSplitOptions.None);
                        decimal? openPrice = null;
                        decimal? closePrice = null;
                        decimal? highPrice = null;
                        decimal? lowPrice = null;
                        int? volume = null;
                        DateTime.TryParse(p[0], out DateTime day);

                        if (decimal.TryParse(p[1], out decimal price))
                        {
                            if (currency == "GBP")
                                price /= 100;
                            openPrice = price;
                        }
                        if (decimal.TryParse(p[2], out price))
                        {
                            if (currency == "GBP")
                                price /= 100;
                            closePrice = price;
                        }
                        if (decimal.TryParse(p[3], out price))
                        {
                            if (currency == "GBP")
                                price /= 100;
                            highPrice = price;
                        }
                        if (decimal.TryParse(p[4], out price))
                        {
                            if (currency == "GBP")
                                price /= 100;
                            lowPrice = price;
                        }

                        if (int.TryParse(p[5], out int vol))
                        {
                            volume = vol;
                        }
                        return new EndOfDayPrice(listingId, day, openPrice, closePrice, highPrice, lowPrice, volume);
                    }).Where(eod => eod.ClosePrice.HasValue);
        }

        public IEnumerable<CurrencyRate> GetRates(IEnumerable<string> data, string symbol)
        {
            return data
                   .Where(d => !string.IsNullOrWhiteSpace(d))
                   .Select(d =>
                   {
                       var p = d.Split(',', StringSplitOptions.None);
                       DateTime.TryParse(p[0], out DateTime day);
                       decimal.TryParse(p[1], out decimal rate);
                       return new CurrencyRate { From = symbol, Day = day, To = "GBP", Rate = rate };
                   });
        }
    }
}
