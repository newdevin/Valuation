using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Valuation.Domain;

namespace Valuation.WorldTradingData.Service
{
    public class AlphaVantageDataService : ITradingDataService
    {

        private readonly Uri baseUri;
        private readonly IApiRepository apiRepository;
        private List<string> tokens;
        private static int index;
        private static readonly object lockObject = new object();

        public AlphaVantageDataService(Uri baseUri, IApiRepository apiRepository)
        {

            this.baseUri = baseUri;
            this.apiRepository = apiRepository;
            GetTokens();
        }
        public void GetTokens()
        {
            if (tokens is null)
            {
                var task = apiRepository.GetTokens("AlphaVantage");
                tokens = task.Result;
            }
        }

        public Uri GetCurrencyRateUri(DateTime? day, string symbol)
        {
            //https://www.alphavantage.co/query?function=FX_DAILY&from_symbol=EUR&to_symbol=USD&apikey=demo&datatype=csv
            var size = "compact";
            if (day is null || ((DateTime.Now.Date - day.Value.Date).TotalDays > 100))
                size = "full";
            var uriString = $"{baseUri}query?function=FX_DAILY&from_symbol={symbol}&to_symbol=GBP&apikey={GetKey()}&datatype=csv&outputsize={size}";

            return new Uri(uriString);

        }

        public Uri GetEndOfDayPriceUri(DateTime? dateTime, string symbol, string suffix)
        {
            //https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=IBM&apikey=demo&datatype=csv&outputsize=compact
            var sym = $"{symbol}";
            if (!string.IsNullOrEmpty(suffix))
                sym = $"{sym}.{suffix}";
            var size = "compact";
            if (dateTime is null || ((DateTime.Now.Date - dateTime.Value.Date).TotalDays > 100))
                size = "full";

            var uriString = $"{baseUri}query?function=TIME_SERIES_DAILY&symbol={sym}&apikey={GetKey()}&datatype=csv&outputsize={size}";
            return new Uri(uriString);

        }

        private string GetKey()
        {
            lock (lockObject)
            {
                if (index >= tokens.Count())
                    index = 0;
                return tokens[index++];
            }
        }

        public IEnumerable<EndOfDayPrice> GetPrices(IEnumerable<string> data, int listingId, string currency)
        {
            //timestamp,open,high,low,close,volume

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
                            highPrice = price;
                        }
                        if (decimal.TryParse(p[3], out price))
                        {
                            if (currency == "GBP")
                                price /= 100;
                            lowPrice = price;
                        }
                        if (decimal.TryParse(p[4], out price))
                        {
                            if (currency == "GBP")
                                price /= 100;
                            closePrice = price;
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
                       decimal.TryParse(p[4], out decimal rate);
                       return new CurrencyRate(0, symbol, "GBP", rate, day);
                   });
        }
    }
}
