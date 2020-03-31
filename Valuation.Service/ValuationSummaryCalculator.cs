using System.Collections.Generic;
using System.Linq;
using Valuation.Domain;

namespace Valuation.Service
{
    public class ValuationSummaryCalculator
    {
        public IEnumerable<ValuationSummary> GetSummary(IEnumerable<BuyTrade> buyTrades, IEnumerable<SellTrade> sellTrades, List<ListingValuation> valuations)
        {
            var summary = valuations.GroupBy(v => v.Day)
                .Select(g =>
                {
                    var totalCostInGbp = buyTrades.Where(bt => bt.BoughtOn <= g.Key).Sum(bt => bt.TotalPaidInGbp);
                    var sellTradesToDate = sellTrades.Where(st => st.SoldOn <= g.Key);
                    var totalSellInGbp = sellTradesToDate.Sum(st => st.TotalReceivedInGbp);
                    var totalValuation = g.Sum(x => x.TotalValueInGbp);
                    var totalRealised = totalSellInGbp - (sellTradesToDate.Select(st => st.BuyTrade).Sum(bt => bt.TotalPaidInGbp));
                    return new ValuationSummary
                    {
                        Day = g.Key,
                        TotalCostInGbp = totalCostInGbp,
                        ValuationInGbp = totalValuation,
                        TotalRealisedInGbp = totalRealised,
                        TotalSellInGbp = totalSellInGbp,
                        TotalProfitInGbp = totalValuation + totalSellInGbp - totalCostInGbp
                    };
                });
            return summary;
        }
    }
}
