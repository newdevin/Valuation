using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Valuation.WorldTradingData.Service;
using Xunit;
using static LanguageExt.Prelude;

namespace WorldTradingData.Service.Test
{
    public class AlphaVantageDataServiceTest
    {
        AlphaVantageDataService AlphaVantageDataService;
        public const string Base_Uri = @"https://alphavantage.co";
        public List<string> Tokens = new List<string> { "LM0o123Ret" };
        
        IApiRepository worldTradingDataRepository;
        public AlphaVantageDataServiceTest()
        {
            worldTradingDataRepository = Substitute.For<IApiRepository>();
            worldTradingDataRepository.GetTokens(Arg.Any<string>()).Returns(Tokens);
            AlphaVantageDataService = new AlphaVantageDataService(new Uri(Base_Uri), worldTradingDataRepository);
        }

        [Fact]
        public void ShouldReturnCorrectUriWhenDateIsNotSpecified()
        {


            string symbol = "ABC";
            string size = "full";
            var uri = AlphaVantageDataService.GetEndOfDayPriceUri(null, symbol, null);

            var query = uri.Query.Replace("?", "");
            Assert.NotNull(query);

            var tokens = GetTokens(query);
            Assert.True(tokens.ContainsKey("datatype"));
            Assert.Equal("csv", tokens["datatype"]);

            Assert.True(tokens.ContainsKey("apikey"));
            Assert.Equal(Tokens.First(), tokens["apikey"]);

            Assert.True(tokens.ContainsKey("symbol"));
            Assert.Equal(symbol, tokens["symbol"]);

            Assert.True(tokens.ContainsKey("outputsize"));
            Assert.Equal(size, tokens["outputsize"]);

            Assert.True(tokens.ContainsKey("function"));
            Assert.Equal("TIME_SERIES_DAILY", tokens["function"]);
                     
        }

        [Fact]
        public void ShouldReturnCorrectUriWhenDateIsSpecified()
        {
            string symbol = "ABC";
            var day = DateTime.Now.AddDays(-10);
            string size = "compact";
            var uri = AlphaVantageDataService.GetEndOfDayPriceUri(day, symbol, null);

            var query = uri.Query.Replace("?", "");
            Assert.NotNull(query);

            var tokens = GetTokens(query);
            Assert.True(tokens.ContainsKey("datatype"));
            Assert.Equal("csv", tokens["datatype"]);

            Assert.True(tokens.ContainsKey("apikey"));
            Assert.Equal(Tokens.First(), tokens["apikey"]);

            Assert.True(tokens.ContainsKey("symbol"));
            Assert.Equal(symbol, tokens["symbol"]);

            Assert.True(tokens.ContainsKey("outputsize"));
            Assert.Equal(size, tokens["outputsize"]);

            Assert.True(tokens.ContainsKey("function"));
            Assert.Equal("TIME_SERIES_DAILY", tokens["function"]);

            

        }

        private IDictionary<string, string> GetTokens(string queryString)
        {
            var tokenDictionary = new Dictionary<string, string>();

            var tokens = queryString.Split("&");
            System.Array.ForEach(tokens, token =>
            {
                var keyValue = token.Split("=");
                tokenDictionary.Add(keyValue[0], keyValue[1]);
            });

            return tokenDictionary;
        }


        [Fact]
        public void ShouldReturnCorrectEndOfDayPrices()
        {
            var curDirectory = Directory.GetCurrentDirectory();
            var pathToDataDirectory = Path.Combine(curDirectory, "data");
            var filePath = Path.Combine(pathToDataDirectory, "endOfDayPrice - alpha.txt");
            var contents = File.ReadAllLines(filePath);
            var data = contents.Skip(1);

            var actual = AlphaVantageDataService.GetPrices(data, 1, "USD");

            Assert.Equal(3, actual.Count());


        }
    }
}
