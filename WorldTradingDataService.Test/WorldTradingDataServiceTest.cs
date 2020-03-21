using System;
using System.Collections.Generic;
using System.Linq;
using Valuation.WorldTradingData.Service;
using Xunit;
using static LanguageExt.Prelude;

namespace WorldTradingData.Service.Test
{
    public class WorldTradingDataServiceTest
    {
        WorldTradingDataService worldTradingDataService;
        public const string Base_Uri = @"https://api.worldtradingdata.com";
        public const string Token = "LM0o123Ret";
        public WorldTradingDataServiceTest()
        {
            worldTradingDataService = new WorldTradingDataService(new Uri(Base_Uri), Token);
        }

        [Fact]
        public void ShouldReturnCorrectUriWhenDateIsNotSpecified()
        {
            string symbol = "ABC";
            var uri = worldTradingDataService.GetEndOfDayPriceUri(None, symbol);
            
            var query = uri.Query.Replace("?","");
            Assert.NotNull(query);

            var tokens = GetTokens(query);
            Assert.True(tokens.ContainsKey("output"));
            Assert.Equal("csv" , tokens["output"]);
            
            Assert.True(tokens.ContainsKey("api_token"));
            Assert.Equal(Token, tokens["api_token"]);

            Assert.True(tokens.ContainsKey("symbol"));
            Assert.Equal(symbol, tokens["symbol"]);

            var uriPathOnly = uri.ToString().Substring(0, uri.ToString().IndexOf("?"));
            Assert.Equal($"{Base_Uri}/api.v1/history", uriPathOnly);
        }

        [Fact]
        public void ShouldReturnCorrectUriWhenDateIsSpecified()
        {
            string symbol = "ABC";
            var dateString = "2020-03-01";
            var uri = worldTradingDataService.GetEndOfDayPriceUri(Some(DateTime.Parse(dateString)), symbol);

            var query = uri.Query.Replace("?", "");
            Assert.NotNull(query);

            var tokens = GetTokens(query);
            Assert.True(tokens.ContainsKey("output"));
            Assert.Equal("csv", tokens["output"]);

            Assert.True(tokens.ContainsKey("api_token"));
            Assert.Equal(Token, tokens["api_token"]);

            Assert.True(tokens.ContainsKey("symbol"));
            Assert.Equal(symbol, tokens["symbol"]);

            Assert.True(tokens.ContainsKey("date_from"));
            Assert.Equal(dateString, tokens["date_from"]);

            var uriPathOnly = uri.ToString().Substring(0, uri.ToString().IndexOf("?"));
            Assert.Equal($"{Base_Uri}/api.v1/history", uriPathOnly);

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
    }
}
