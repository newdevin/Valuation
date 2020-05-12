using Microsoft.EntityFrameworkCore;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;
using Valuation.Repository.Entities;
using Valuation.Repository.Mapper;
using Valuation.Service;
using Valuation.Service.Repository;

namespace Valuation.Repository
{
    public class ValuationRepository : IValuationRepository
    {
        private readonly PicassoDbContext context;
        private readonly IObjectMapper mapper;

        public ValuationRepository(PicassoDbContext context, IObjectMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<PortfolioValuation> GetPortfolioValuation(DateTime onDay)
        {
            var day = onDay.Date;
            var summaries = await context.ValuationSummaries.Where(vs => vs.Day == day.Date || vs.Day == day.AddDays(-1).Date).ToListAsync();
            var summary1 = summaries.FirstOrDefault(e => e.Day == day);
            var summary2 = summaries.FirstOrDefault(e => e.Day == day.AddDays(-1).Date);

            return new PortfolioValuation
            {
                ValuationSummary = summary1,
                TotalCostChange = summary1.TotalCostInGbp - summary2.TotalCostInGbp,
                TotalProfitChange = summary1.TotalProfitInGbp - summary2.TotalProfitInGbp,
                TotalRealisedChange = summary1.TotalRealisedInGbp - summary2.TotalRealisedInGbp,
                TotalSellChanged = summary1.TotalSellInGbp - summary2.TotalSellInGbp,
                TotalValuationChange = summary1.ValuationInGbp - summary2.ValuationInGbp,
                TotalCashInvestedInGbp = summary1.TotalCashInvestedInGbp - summary2.TotalCashInvestedInGbp,
                TotalCashWithdrawnInGbp = summary1.TotalCashWithdrawnInGbp - summary2.TotalCashWithdrawnInGbp
            };

        }

        public async Task<IEnumerable<ListingValuation>> GetValuations(DateTime day)
        {
            var entities = await context.Valuations.AsNoTracking().Where(v => v.Day == day)
                .ToListAsync();

            return mapper.MapTo<ListingValuation>(entities);
        }

        public async Task<IEnumerable<ValuationSummary>> GetValuationSummary()
        {
            return await context.ValuationSummaries.AsNoTracking().OrderByDescending(s => s.Day).ToListAsync();
        }

        public async Task Save(IEnumerable<ListingValuation> valuations, IEnumerable<ValuationSummary> summary)
        {
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Valuation");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE ValuationSummary");
            await context.SaveChangesAsync();

            var entities = mapper.MapTo<ListingValuationEntity>(valuations);
            context.Valuations.AddRange(entities);
            context.ValuationSummaries.AddRange(summary);
            await context.SaveChangesAsync();
        }

        public async Task ValuePortfolio()
        {
            await context.Database.ExecuteSqlRawAsync("EXEC dbo.FullValuation");
        }
    }
}
