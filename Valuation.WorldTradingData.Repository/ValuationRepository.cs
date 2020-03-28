using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;
using Valuation.Repository.Entities;
using Valuation.Service.Repository;

namespace Valuation.Repository
{
    public class ValuationRepository : IValuationRepository
    {
        private readonly PicassoDbContext context;

        public ValuationRepository(PicassoDbContext context)
        {
            this.context = context;
        }
        public async Task Save(IEnumerable<ListingValuation> valuations)
        {
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Valuation");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE ValuationSummary");
            var summary = valuations.GroupBy(v => v.Day)
                .Select(g =>
                {
                    var totalCostInGbp = context.BuyTrades.Where(bt => bt.BoughtOn <= g.Key).Sum(bt => bt.TotalPaidInGbp);
                    var sellTradesToDate = context.SellTrades.Include(st => st.BuyTrade).Where(st => st.SoldOn <= g.Key);
                    var totalSellInGbp = sellTradesToDate.Sum(st => st.TotalReceivedInGbp);
                    var totalValuation = g.Sum(x => x.TotalValueInGbp);
                    var totalRealised = totalSellInGbp - (sellTradesToDate.Select(st => st.BuyTrade).Sum(bt => bt.TotalPaidInGbp));
                    return new ValuationSummaryEntity
                    {
                        Day = g.Key,
                        TotalCostInGBP = totalCostInGbp,
                        ValuationInGbp = totalValuation,
                        TotalRealisedInGBP = totalRealised,
                        TotalSellInGBP = totalSellInGbp,
                        TotalProfitInGBP = totalValuation + totalSellInGbp - totalCostInGbp
                    };
                });

            context.Valuations.AddRange(valuations);
            context.ValuationSummaries.AddRange(summary);
            await context.SaveChangesAsync();
        }
    }
}
