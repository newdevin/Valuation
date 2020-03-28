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
                new ListingValuation { Id =1, Currency = "GBP", Day = day1, ListingId = 1, Price =1 , Quantity = 1000, Rate =1, TotalValue = 1000, TotalValueInGbp = 1000   },
                new ListingValuation { Id =2, Currency = "GBP", Day = day2, ListingId = 1, Price =1.1m , Quantity = 1000, Rate =1, TotalValue = 1100, TotalValueInGbp = 1100   },
                new ListingValuation { Id =4, Currency = "USD", Day = day1, ListingId = 2, Price =1m , Quantity = 100, Rate = 0.5m, TotalValue = 100, TotalValueInGbp = 50   },
                new ListingValuation { Id =5, Currency = "USD", Day = day2, ListingId = 2, Price =2m , Quantity = 100, Rate = 0.5m, TotalValue = 200, TotalValueInGbp = 100   },
                new ListingValuation { Id =6, Currency = "USD", Day = day3 , ListingId = 2, Price =2.1m , Quantity = 100, Rate = 0.5m, TotalValue = 210, TotalValueInGbp = 105 }
            };

            var actual = summaryCalculator.GetSummary(buyTrades, sellTrades, valuations);

            Assert.Equal(3, actual.Count());
            var first = actual.First(a => a.Day == day1);
            Assert.Equal(1050, first.ValuationInGbp);
            Assert.Equal(1106, first.TotalCostInGBP);
            Assert.Equal(-56, first.TotalProfitInGBP);
            Assert.Equal(0, first.TotalSellInGBP);
            Assert.Equal(0, first.TotalRealisedInGBP);

            var second = actual.First(a => a.Day == day2);
            Assert.Equal(1200, second.ValuationInGbp);
            Assert.Equal(1106, second.TotalCostInGBP);
            Assert.Equal(0, second.TotalSellInGBP);
            Assert.Equal(94, second.TotalProfitInGBP);
            Assert.Equal(0, second.TotalRealisedInGBP);


            var third = actual.First(a => a.Day == day3);
            Assert.Equal(105, third.ValuationInGbp);
            Assert.Equal(1106, third.TotalCostInGBP);
            Assert.Equal(1500, third.TotalSellInGBP);
            Assert.Equal(499, third.TotalProfitInGBP);
            Assert.Equal(499, third.TotalRealisedInGBP);


        }
    }
}
