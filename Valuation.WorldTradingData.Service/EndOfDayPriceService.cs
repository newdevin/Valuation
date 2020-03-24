using System;
using System.Threading.Tasks;
using Valuation.Service;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using Valuation.Domain;
using System.Collections.Concurrent;

namespace Valuation.WorldTradingData.Service
{
    public class EndOfDayPriceService : IEndOfDayPriceService
    {
        private readonly IWorldTradingDataService worldTradingDataService;
        private readonly IEndOfDayRepository endOfDayRepository;
        private readonly IListingService listingService;
        private readonly IHttpClientFactory httpClientFactory;

        public EndOfDayPriceService(IWorldTradingDataService worldTradingDataService,
            IEndOfDayRepository endOfDayRepository, IListingService listingService, IHttpClientFactory httpClientFactory)
        {
            this.worldTradingDataService = worldTradingDataService;
            this.endOfDayRepository = endOfDayRepository;
            this.listingService = listingService;
            this.httpClientFactory = httpClientFactory;
        }
        public async Task DownloadEndOfDayPrices()
        {
            var listingsWithDate = await listingService.GetActiveListingWithLastEodPriceDateTime();
            var queue = new ConcurrentQueue<EndOfDayPrice>();
            var task = listingsWithDate.Select(listingWithDate =>
            {
                return Task.Run(async () =>
                {
                    var uri = worldTradingDataService.GetEndOfDayPriceUri(listingWithDate.Item2?.AddDays(1), listingWithDate.Item1.Symbol, listingWithDate.Item1.Suffix);
                    var endOfDayPrices = await GetEndOfDayPrice(listingWithDate.Item1.Id, uri);
                    foreach (var eodPrice in endOfDayPrices)
                    {
                        queue.Enqueue(eodPrice);
                    }
                });
            });
            await Task.WhenAll(task);
            await endOfDayRepository.Save(queue.Select(eodPrice => eodPrice));
        }

        private async Task<IEnumerable<EndOfDayPrice>> GetEndOfDayPrice(int listingId, Uri uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            using var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                //Date,Open,Close,High,Low,Volume
                var allLines = data.Split("\n");
                var dataLines = allLines.Skip(1);
                return dataLines
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
                        openPrice = price;
                    }
                    if (decimal.TryParse(p[2], out price))
                    {
                        closePrice = price;
                    }
                    if (decimal.TryParse(p[3], out price))
                    {
                        highPrice = price;
                    }
                    if (decimal.TryParse(p[4], out price))
                    {
                        lowPrice = price;
                    }

                    if (int.TryParse(p[5], out int vol))
                    {
                        volume = vol;
                    }

                    return new EndOfDayPrice(listingId, day, openPrice, closePrice, highPrice, lowPrice, volume);
                });
            }
            return Array.Empty<EndOfDayPrice>();
        }


    }
}
