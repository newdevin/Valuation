using LanguageExt;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Valuation.Domain;
using Valuation.Service;

namespace Valuation.WorldTradingData.Service
{
    public class AlphaVantageQuoteService : IQuoteService
    {
        private readonly ILogger<AlphaVantageQuoteService> logger;
        private readonly ITradingDataService tradingDataService;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly int? delay;

        public AlphaVantageQuoteService(ILogger<AlphaVantageQuoteService> logger, ITradingDataService tradingDataService,
            IHttpClientFactory httpClientFactory, int? delay)
        {
            this.logger = logger;
            this.tradingDataService = tradingDataService;
            this.httpClientFactory = httpClientFactory;
            this.delay = delay;
        }
        public async Task<IEnumerable<Quote>> GetQuotes(IEnumerable<Listing> listings)
        {
            var queue = new ConcurrentQueue<Quote>();
            var tasks = listings.Select(listing =>
            {
                return new Task(async () =>
               {
                   var uri = tradingDataService.GetQuoteUri(listing.Symbol, listing.Suffix);
                   var quote = await GetQuote(listing, uri);
                   queue.Enqueue(quote);
               });
            });

            foreach (var tsk in tasks)
            {
                tsk.RunSynchronously();
                if (delay.HasValue)
                    await Task.Delay(TimeSpan.FromSeconds(delay.Value));
            }
            return queue.ToList();
        }

        private async Task<Quote> GetQuote(Listing listing, Uri uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            using var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();

                var allLines = data.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                var dataLines = allLines.Skip(1);
                return tradingDataService.GetQuote(dataLines.FirstOrDefault(), listing);
            }
            else
                logger.LogWarning($"Response for: {uri} was : {response.StatusCode}");
            return Quote.Empty();
        }
    }
}
