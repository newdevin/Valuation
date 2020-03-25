using LanguageExt;
using NSubstitute;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;
using Valuation.Service;
using Valuation.WorldTradingData.Service;
using Xunit;

namespace WorldTradingData.Service.Test
{
    public class EndOfDayServiceTest
    {
        EndOfDayPriceDownloadService endOfDayService;
        IWorldTradingDataService worldTradingDataService;
        IEndOfDayPriceRepository endOfDayRepository;
        IListingService listingService;
        IHttpClientFactory httpClientFactory;
        public EndOfDayServiceTest()
        {
            worldTradingDataService = Substitute.For<IWorldTradingDataService>();
            endOfDayRepository = Substitute.For<IEndOfDayPriceRepository>();
            listingService = Substitute.For<IListingService>();
            httpClientFactory = Substitute.For<IHttpClientFactory>();

            endOfDayService = new EndOfDayPriceDownloadService(worldTradingDataService, endOfDayRepository, listingService, httpClientFactory);
        }

        private IEnumerable<Tuple<Listing, DateTime?>> GetListings()
        {
            var company = new Company(1, "SNAP", "Dummy company");
            var exchange = new Exchange("NYSE");
            var currency = new Currency("USD");
            var listing = new Listing(1, company, exchange, currency, "SNAP", "");

            return new List<Tuple<Listing, DateTime?>> { Tuple.Create(listing,(DateTime?) new DateTime(2020, 03, 21)) };

        }


        [Fact]
        public async Task CorrectEndOfDayPricesAreReturned()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), @"Data\endOfDayPrice.txt");
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(File.ReadAllText(path))
            });
            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);

            httpClientFactory.CreateClient().Returns(fakeHttpClient);
            listingService.GetActiveListingWithLastEodPriceDateTime().Returns(GetListings());
            worldTradingDataService.GetEndOfDayPriceUri(Arg.Any<DateTime>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new Uri("http://someuri"));

            await endOfDayService.DownloadEndOfDayPrices();

            _ = endOfDayRepository.Received().Save(Arg.Is<IEnumerable<EndOfDayPrice>>(l => l.Count() == 3));

        }
    }
}
