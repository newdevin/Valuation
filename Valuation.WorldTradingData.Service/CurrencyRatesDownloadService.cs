using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;
using Valuation.Service;

namespace Valuation.WorldTradingData.Service
{
    public class CurrencyRatesDownloadService : ICurrencyRatesDownloadService
    {
        private readonly IWorldTradingDataService worldTradingDataService;
        private readonly ICurrencyRateService currencyRateService;
        private readonly IHttpClientFactory httpClientFactory;

        public CurrencyRatesDownloadService(IWorldTradingDataService worldTradingDataService,
            ICurrencyRateService currencyRateService,
            IHttpClientFactory httpClientFactory)
        {
            this.worldTradingDataService = worldTradingDataService;
            this.currencyRateService = currencyRateService;
            this.httpClientFactory = httpClientFactory;
        }
        public async Task DownloadCurrenctRates()
        {
            var currenciesWithDate = await currencyRateService.GetActiveCurrenciesWithLastDownloadedDate();
            var queue = new ConcurrentQueue<CurrencyRate>();
            var task = currenciesWithDate.Select(c =>
            {
                return Task.Run(async () =>
                {
                    var uri = worldTradingDataService.GetCurrencyRateUri(c.Item2?.AddDays(1), c.Item1.Symbol);
                    var currencyRates = await GetCurrencyRates(c.Item1.Symbol, uri);
                    foreach(var rate in currencyRates)
                    {
                        queue.Enqueue(rate);
                    }
                });
            });

            await Task.WhenAll(task);
            await currencyRateService.Save(queue.Select(rate => rate));

        }

        private async Task<IEnumerable<CurrencyRate>> GetCurrencyRates(string symbol, Uri uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            using var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                //Date,rate
                var allLines = data.Split("\n");
                var dataLines = allLines.Skip(1);
                return dataLines
                    .Where(d => !string.IsNullOrWhiteSpace(d))
                    .Select(d =>
                    {
                        var p = d.Split(',', StringSplitOptions.None);
                        DateTime.TryParse(p[0], out DateTime day);
                        decimal.TryParse(p[1], out decimal rate);
                        return new CurrencyRate { From = symbol, Day = day, To = "GBP", Rate = rate };
                    });
            }
            return Array.Empty<CurrencyRate>();
        }
    }
}
