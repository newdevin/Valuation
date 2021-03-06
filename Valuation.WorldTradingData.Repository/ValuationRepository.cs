﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ValuationRepository> _logger;

        public ValuationRepository(PicassoDbContext context, IObjectMapper mapper, ILogger<ValuationRepository> logger)
        {
            this.context = context;
            this.mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ListingValuation>> GetValuations(DateTime day)
        {
            var entities = await context.Valuations
                .AsNoTracking()
                .Where(v => v.Day == day)
                .ToListAsync();

            return mapper.MapTo<ListingValuation>(entities);
        }

        public async Task<ValuationSummary> GetValuationSummaryOnDay(DateTime day)
        {
            _logger.LogInformation($"Getting valuation summary for day: {day:yyyy-MM-dd}");
            var entity = await context.ValuationSummaries
                .FirstOrDefaultAsync(s => s.Day.Date == day.Date);

            return mapper.MapTo<ValuationSummary>(entity);
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
            context.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));
            await context.Database.ExecuteSqlRawAsync("EXEC dbo.FullValuation");
        }
    }
}
