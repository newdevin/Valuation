using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;
using Valuation.Repository.Entities;
using Valuation.Service;
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

        public async Task<IEnumerable<ValuationSummary>> GetValuationSummary()
        {
            return await context.ValuationSummaries.OrderByDescending(s => s.Day).ToListAsync();
        }

        public async Task Save(IEnumerable<ListingValuation> valuations, IEnumerable<ValuationSummary> summary)
        {
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Valuation");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE ValuationSummary");
            
            context.Valuations.AddRange(valuations);
            context.ValuationSummaries.AddRange(summary);
            await context.SaveChangesAsync();
        }
    }
}
