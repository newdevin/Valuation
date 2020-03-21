using System;
using System.Threading.Tasks;
using Valuation.Service;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using Valuation.Domain;

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
        public async Task DownloadEndOfDayPrices(DateTime endOfDay)
        {
            var listings = await listingService.GetActiveListing();

            var task = listings.Select(listing =>
            {
                return Task.Run(async () =>
                {
                    var uri = worldTradingDataService.GetEndOfDayPriceUri(endOfDay, listing.Symbol);
                    var endOfDayPrices = await GetEndOfDayPrice(listing.Id, uri);
                    await endOfDayRepository.Save(endOfDayPrices);
                });
            });
            await Task.WhenAll(task);
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
                return dataLines.Select(d =>
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
                        closePrice = price;
                    }

                    if (int.TryParse(p[5], out int vol))
                    {
                        volume = vol;
                    }

                    return EndOfDayPrice.Create(listingId, day, openPrice, closePrice, highPrice, lowPrice, volume);
                });
            }
            return Array.Empty<EndOfDayPrice>();
        }


    }
}
