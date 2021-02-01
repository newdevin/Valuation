using System;
using System.Threading.Tasks;
using Valuation.Service;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using Valuation.Domain;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Valuation.WorldTradingData.Service
{
    public class EndOfDayPriceDownloadService : IEndOfDayPriceDownloadService
    {
        private readonly ILogger logger;
        private readonly ITradingDataService tradingDataService;
        private readonly IEndOfDayPriceRepository endOfDayRepository;
        private readonly IListingService listingService;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly int? delay;

        public EndOfDayPriceDownloadService(ILogger<EndOfDayPriceDownloadService> logger, ITradingDataService tradingDataService,
            IEndOfDayPriceRepository endOfDayRepository, IListingService listingService, IHttpClientFactory httpClientFactory, int? delay)
        {
            this.logger = logger;
            this.tradingDataService = tradingDataService;
            this.endOfDayRepository = endOfDayRepository;
            this.listingService = listingService;
            this.httpClientFactory = httpClientFactory;
            this.delay = delay;
        }
        public async Task DownloadEndOfDayPrices()
        {
            var listingsWithDate = await listingService.GetActiveListingWithLastEodPriceDateTime();
            var queue = new ConcurrentQueue<EndOfDayPrice>();
            var tasks = listingsWithDate.Select(listingWithDate =>
            {
                 return Task.Run(async () => 
               // return new Task(async () => //*** This is for AlphaVantage Only ***//
                {
                    var uri = tradingDataService.GetEndOfDayPriceUri(listingWithDate.Item2?.AddDays(1), listingWithDate.Item1.Symbol, listingWithDate.Item1.Suffix);
                    var endOfDayPrices = await GetEndOfDayPrice(listingWithDate.Item1.Id, uri, listingWithDate.Item1.Currency.Symbol);
                    foreach (var eodPrice in endOfDayPrices)
                    {
                        queue.Enqueue(eodPrice);
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
            await endOfDayRepository.Save(queue.Select(eodPrice => eodPrice));
        }
        private async Task<IEnumerable<EndOfDayPrice>> GetEndOfDayPrice(int listingId, Uri uri, string currency)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            
            using var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
               // logger.LogInformation(data);
                var allLines = data.Split("\n");
                var dataLines = allLines.Skip(1);
                return tradingDataService.GetPrices(dataLines, listingId, currency);
            }
            else
                logger.LogWarning($"Resposnse for: {uri} was : {response.StatusCode}");
            return Array.Empty<EndOfDayPrice>();
        }

    }
}
