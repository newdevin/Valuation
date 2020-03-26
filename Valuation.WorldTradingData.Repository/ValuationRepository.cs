using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;
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

            context.Valuations.AddRange(valuations);
            await context.SaveChangesAsync();
        }
    }
}
