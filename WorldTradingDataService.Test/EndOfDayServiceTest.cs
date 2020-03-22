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
        EndOfDayPriceService endOfDayService;
        IWorldTradingDataService worldTradingDataService;
        IEndOfDayRepository endOfDayRepository;
        IListingService listingService;
        IHttpClientFactory httpClientFactory;
        public EndOfDayServiceTest()
        {
            worldTradingDataService = Substitute.For<IWorldTradingDataService>();
            endOfDayRepository = Substitute.For<IEndOfDayRepository>();
            listingService = Substitute.For<IListingService>();
            httpClientFactory = Substitute.For<IHttpClientFactory>();

            endOfDayService = new EndOfDayPriceService(worldTradingDataService, endOfDayRepository, listingService, httpClientFactory);
        }

        private IEnumerable<Listing> GetListings()
        {
            var company = new Company(1, "SNAP", "Dummy company");
            var exchange = new Exchange("NYSE");
            var currency = new Currency("USD");
            var listing = new Listing(1, company, exchange, currency, "SNAP", "");

            return new List<Listing> { listing };

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
            listingService.GetActiveListing().Returns(GetListings());
            worldTradingDataService.GetEndOfDayPriceUri(Arg.Any<DateTime>(), Arg.Any<string>())
                .Returns(new Uri("http://someuri"));

            await endOfDayService.DownloadEndOfDayPrices(DateTime.Now);

            _ = endOfDayRepository.Received().Save(Arg.Is<IEnumerable<EndOfDayPrice>>(l => l.Count() == 3));

        }
    }
}
