using Microsoft.EntityFrameworkCore;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuation.Common;
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

        public async Task<PortfolioValuationSummary> GetPortfolioValuation(DateTime onDay)
        {
            var day = onDay.Date;
            if (onDay.IsWeekend())
            {
                day = (onDay.DayOfWeek == DayOfWeek.Saturday) ? onDay.Date.AddDays(-1) : onDay.Date.AddDays(-2);
            }

            var previousDay = day.AddDays(-1).Date;
            var summaries = await context.ValuationSummaries
                .Where(vs => vs.Day == day.Date || vs.Day == previousDay)
                .ToListAsync();
            var currentSummary = summaries.FirstOrDefault(e => e.Day == day);
            var previousDaySummary = summaries.FirstOrDefault(e => e.Day == previousDay);


            var prices = await context.EndOfDayPrices
                .Include(eod => eod.Listing.Currency)
                .Where(eod => eod.Day == day || eod.Day == previousDay)
                .ToListAsync();

            var currentPrices = prices.Where(p => p.Day == day).ToList();
            var previousPrices = prices.Where(p => p.Day == previousDay).ToList();

            var listingValuationSummaries1 = previousPrices
                .GroupJoin(currentPrices, p => p.ListingId, c => c.ListingId,
                    (p, c) => new { Previous = p, Current = c.DefaultIfEmpty() })
                .Select(x =>
                {
                    return new ListingValuationSummary
                    {
                        Listing = mapper.MapTo<Listing>(x?.Previous?.Listing),
                        Day = day,
                        CurrentShareValue = x.Current?.First()?.Close,
                        PreviousBusinessDayShareValue = x.Previous?.Close,
                        Currency = mapper.MapTo<Currency>(x.Previous?.Listing?.Currency)
                    };
                });

            var listingValuationSummaries2 = currentPrices
                .GroupJoin(previousPrices, c => c.ListingId, p => p.ListingId,
                    (c, p) => new { Current = c , Previous = p.DefaultIfEmpty()})
                .Select(x =>
                {
                    return new ListingValuationSummary
                    {
                        Listing = mapper.MapTo<Listing>(x?.Current?.Listing),
                        Day = day,
                        CurrentShareValue = x.Current?.Close,
                        PreviousBusinessDayShareValue = x.Previous?.First()?.Close,
                        Currency = mapper.MapTo<Currency>(x.Current?.Listing?.Currency)
                    };
                });
            var listingValuationSummaries = listingValuationSummaries1
                .Union(listingValuationSummaries2)
                .Distinct()
                .ToList();

            return new PortfolioValuationSummary
            {
                ValuationSummary = currentSummary,
                ListingValuationSummaries = listingValuationSummaries,
                TotalCostChange = currentSummary.TotalCostInGbp - previousDaySummary.TotalCostInGbp,
                TotalProfitChange = currentSummary.TotalProfitInGbp - previousDaySummary.TotalProfitInGbp,
                TotalRealisedChange = currentSummary.TotalRealisedInGbp - previousDaySummary.TotalRealisedInGbp,
                TotalSellChanged = currentSummary.TotalSellInGbp - previousDaySummary.TotalSellInGbp,
                TotalValuationChange = currentSummary.ValuationInGbp - previousDaySummary.ValuationInGbp,
                TotalCashInvestedInGbp = currentSummary.TotalCashInvestedInGbp - previousDaySummary.TotalCashInvestedInGbp,
                TotalCashWithdrawnInGbp = currentSummary.TotalCashWithdrawnInGbp - previousDaySummary.TotalCashWithdrawnInGbp
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
