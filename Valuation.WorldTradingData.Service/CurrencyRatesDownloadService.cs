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
        private readonly ITradingDataService tradingDataService;
        private readonly ICurrencyRateService currencyRateService;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly int? delay;

        public CurrencyRatesDownloadService(ITradingDataService tradingDataService,
            ICurrencyRateService currencyRateService,
            IHttpClientFactory httpClientFactory, int? delay)
        {
            this.tradingDataService = tradingDataService;
            this.currencyRateService = currencyRateService;
            this.httpClientFactory = httpClientFactory;
            this.delay = delay;
        }
        public async Task DownloadCurrenctRates()
        {
            var currenciesWithDate = await currencyRateService.GetActiveCurrenciesWithLastDownloadedDate();
            var queue = new ConcurrentQueue<CurrencyRate>();
            var tasks = currenciesWithDate.Select(c =>
            {
                return Task.Run(async () =>
                //return new Task(async () =>
                {
                    var uri = tradingDataService.GetCurrencyRateUri(c.Item2?.AddDays(1), c.Item1.Symbol);
                    var currencyRates = await GetCurrencyRates(c.Item1.Symbol, uri);
                    foreach(var rate in currencyRates)
                    {
                        queue.Enqueue(rate);
                    }
                });
            });

            ////******* AlphaVantage specific code start********//
            ////AlphaVantage will only allow 5 call per minute
            //foreach (var tsk in tasks)
            //{
            //    tsk.RunSynchronously();
            //    if (delay.HasValue)
            //        await Task.Delay(TimeSpan.FromSeconds(delay.Value));
            //}
            ////******* AlphaVantage specific code end********//
            await Task.WhenAll(tasks);
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
                return tradingDataService.GetRates(dataLines, symbol);
            }
            return Array.Empty<CurrencyRate>();
        }
    }
}
