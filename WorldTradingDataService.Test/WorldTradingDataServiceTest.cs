using System;
using Valuation.WorldTradingData.Service;
using Xunit;
using static LanguageExt.Prelude;

namespace WorldTradingData.Service.Test
{
    public class WorldTradingDataServiceTest
    {
        WorldTradingDataService worldTradingDataService;
        public const string Base_Uri = @"https://https://api.worldtradingdata.com";
        public const string Token = "LM0o123Ret";
        public WorldTradingDataServiceTest()
        {
            worldTradingDataService = new WorldTradingDataService(new Uri(Base_Uri), Token);
        }

        [Fact]
        public void UriWithNoDateIsCorrect()
        {
            string symbol = "ABC";
            var uri = worldTradingDataService.GetEndOfDayPriceUri(None, symbol);
            var query = uri.Query;
            Assert.NotNull(query);
        }
    }
}
