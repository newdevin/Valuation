using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Valuation.Domain;
using Valuation.Service.Repository;
using Xunit;

namespace Valuation.Service.Test
{
    public class ValuationServiceTest
    {
        private readonly IEndOfDayPriceService endOfDayPriceService;
        private readonly ICurrencyRateService currencyRateService;
        private readonly IListingService listingService;
        private readonly IValuationRepository valuationRepository;
        ValuationService valuationService;

        public ValuationServiceTest()
        {
            endOfDayPriceService = Substitute.For<IEndOfDayPriceService>();
            currencyRateService = Substitute.For<ICurrencyRateService>();
            listingService = Substitute.For<IListingService>();
            valuationRepository = Substitute.For<IValuationRepository>();

            valuationService = new ValuationService(endOfDayPriceService, currencyRateService, listingService, valuationRepository);
        }
        [Fact]
        public async Task ShouldReturnCorrectValuationForGBPCurrency()
        {
            var company = new Company(1, "Abcam", "Abcam ltd.");
            var exchange = new Exchange("LSE");
            var currency = new Currency("GBP");
            var symbol = "ABC";
            var suffix = "L";

            var listingId = 1;

            var listing = new Listing(listingId, company, exchange, currency, symbol, suffix);

            var listingDate = new DateTime(2020, 10, 10);
            var listingVolume = new ListingVolume(1, listing, 100, listingDate);
            var listingVolume2 = new ListingVolume(1, listing, 200, listingDate.AddDays(2));
            var listingVolume3 = new ListingVolume(1, listing, 0, listingDate.AddDays(3));

            var eodPrices = new List<EndOfDayPrice>
            {
                new EndOfDayPrice(listingId,listingVolume.Day,1,1,1,1,1000),
                new EndOfDayPrice(listingId,listingVolume2.Day,1,2,1,1,1000),
            };
            endOfDayPriceService.GetEndOfDayPriceSince(listingDate).Returns(eodPrices);

            var currencyRates = new List<CurrencyRate> { };
            currencyRateService.GetCurencyRatesSince(listingDate).Returns(currencyRates);

            listingService.GetListingVolumes().Returns(new List<ListingVolume>
            {
                listingVolume,
                listingVolume2,
                listingVolume3
            });

            IEnumerable<ListingValuation> valuations = null;
            await valuationRepository.Save(Arg.Do<IEnumerable<ListingValuation>>(vals => valuations = vals));

            await valuationService.ValuePortfolio(listingVolume3.Day);

            Assert.Equal(3, valuations.Count());
            Assert.Equal(100m, valuations.First(v => v.Day == listingVolume.Day).TotalValueInGbp);
            Assert.Equal(100m, valuations.First(v => v.Day == listingVolume.Day.AddDays(1)).TotalValueInGbp);
            Assert.Equal(400m, valuations.First(v => v.Day == listingVolume.Day.AddDays(2)).TotalValueInGbp);

        }

        [Fact]
        public async Task ShouldReturnCorrectValuationForNonBPCurrency()
        {
            var company = new Company(1, "YMAB", "Ymab ltd.");
            var exchange = new Exchange("NYSE");
            var currency = new Currency("USD");
            var symbol = "YMAB";
            var suffix = "";

            var listingId = 1;

            var listing = new Listing(listingId, company, exchange, currency, symbol, suffix);

            var listingDate = new DateTime(2020, 10, 10);
            var listingVolume = new ListingVolume(1, listing, 100, listingDate);
            var listingVolume2 = new ListingVolume(1, listing, 200, listingDate.AddDays(2));

            var eodPrices = new List<EndOfDayPrice>
            {
                new EndOfDayPrice(listingId,listingVolume.Day,1,1,1,1,1000),
                new EndOfDayPrice(listingId,listingVolume2.Day,1,2,1,1,1000)
            };
            endOfDayPriceService.GetEndOfDayPriceSince(listingDate).Returns(eodPrices);

            var currencyRates = new List<CurrencyRate> {
            new CurrencyRate { Day = listingDate, From = "USD", To = "GBP" , Id =1, Rate = 0.50m},
            new CurrencyRate { Day = listingDate.AddDays(1), From = "USD", To = "GBP" , Id =1, Rate = 0.50m},
            new CurrencyRate { Day = listingDate.AddDays(2), From = "USD", To = "GBP" , Id =1, Rate = 1m}
            };
            currencyRateService.GetCurencyRatesSince(listingDate).Returns(currencyRates);

            listingService.GetListingVolumes().Returns(new List<ListingVolume> { listingVolume });

            IEnumerable<ListingValuation> valuations = null;
            await valuationRepository.Save(Arg.Do<IEnumerable<ListingValuation>>(vals => valuations = vals));

            await valuationService.ValuePortfolio(listingVolume2.Day);


            Assert.Equal(3, valuations.Count());
            Assert.Equal(50m, valuations.First(v => v.Day == listingVolume.Day).TotalValueInGbp);
            Assert.Equal(50m, valuations.First(v => v.Day == listingVolume.Day.AddDays(1)).TotalValueInGbp);
            Assert.Equal(200m,valuations.First(v => v.Day == listingVolume.Day.AddDays(2)).TotalValueInGbp);

        }

        [Fact]
        public async Task ShouldReturnCorrectValuationSummary()
        {

        }
    }
}
