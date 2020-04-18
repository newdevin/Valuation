using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Valuation.Domain;
using Xunit;

namespace Valuation.Service.Test
{
    public class ValuationSummaryTest
    {
        [Fact]
        public void ShouldReturnCorrectValuationSummaries()
        {
            var summaryCalculator = new ValuationSummaryCalculator();
            var day1 = new DateTime(2020, 10, 10);
            var day2 = new DateTime(2020, 10, 11);
            var day3 = new DateTime(2020, 10, 12);
            var company1 = new Company(1, "Abc", null);
            var currency1 = new Currency("USD");
            var exchange1 = new Exchange("NYSE");

            var company2 = new Company(1, "Vec", null);
            var currency2 = new Currency("GBP");
            var exchange2 = new Exchange("LSE");

            var listing1 = new Listing(1, company1, exchange1, currency1, "ABC", null);
            var listing2 = new Listing(2, company2, exchange2, currency2, "VEC", "L");

            var buyTrades = new List<BuyTrade>
            {
                new BuyTrade {Id =1, BoughtOn = day1,PricePerShare =1, ListingId =1 , PricePerShareCurrency = "GBP", Quantity = 1000, TotalPaidInGbp = 1001 },
                new BuyTrade {Id =2, BoughtOn = day1,PricePerShare =1, ListingId =2 , PricePerShareCurrency = "USD", Quantity = 100, TotalPaidInGbp = 105 }
            };

            var sellTrades = new List<SellTrade>
            {
                new SellTrade { Id = 1, ListingId = 1, BuyTrade = buyTrades.First(), BuyTradeId = 1, PricePerShare = 2, PricePerShareCurrency ="GBP", Quantity = 1000, SoldOn = day3, TotalReceivedInGbp = 1500 }
            };

            var valuations = new List<ListingValuation>
            {
                new ListingValuation { Id =1, Currency = "GBP", Day = day1, Listing = listing1, Price =1 , Quantity = 1000, Rate =1, TotalValue = 1000, TotalValueInGbp = 1000   },
                new ListingValuation { Id =2, Currency = "GBP", Day = day2, Listing = listing1, Price =1.1m , Quantity = 1000, Rate =1, TotalValue = 1100, TotalValueInGbp = 1100   },
                new ListingValuation { Id =4, Currency = "USD", Day = day1, Listing = listing2, Price =1m , Quantity = 100, Rate = 0.5m, TotalValue = 100, TotalValueInGbp = 50   },
                new ListingValuation { Id =5, Currency = "USD", Day = day2, Listing = listing2, Price =2m , Quantity = 100, Rate = 0.5m, TotalValue = 200, TotalValueInGbp = 100   },
                new ListingValuation { Id =6, Currency = "USD", Day = day3 ,Listing = listing2, Price =2.1m , Quantity = 100, Rate = 0.5m, TotalValue = 210, TotalValueInGbp = 105 }
            };

            var actual = summaryCalculator.GetSummary(buyTrades, sellTrades, valuations);

            Assert.Equal(3, actual.Count());
            var first = actual.First(a => a.Day == day1);
            Assert.Equal(1050, first.ValuationInGbp);
            Assert.Equal(1106, first.TotalCostInGbp);
            Assert.Equal(-56, first.TotalProfitInGbp);
            Assert.Equal(0, first.TotalSellInGbp);
            Assert.Equal(0, first.TotalRealisedInGbp);

            var second = actual.First(a => a.Day == day2);
            Assert.Equal(1200, second.ValuationInGbp);
            Assert.Equal(1106, second.TotalCostInGbp);
            Assert.Equal(0, second.TotalSellInGbp);
            Assert.Equal(94, second.TotalProfitInGbp);
            Assert.Equal(0, second.TotalRealisedInGbp);


            var third = actual.First(a => a.Day == day3);
            Assert.Equal(105, third.ValuationInGbp);
            Assert.Equal(1106, third.TotalCostInGbp);
            Assert.Equal(1500, third.TotalSellInGbp);
            Assert.Equal(499, third.TotalProfitInGbp);
            Assert.Equal(499, third.TotalRealisedInGbp);


        }
    }
}
