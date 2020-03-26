using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;
using Valuation.Service;
using Valuation.Infrastructure;

namespace Valuation.Repository
{
    public class CurrencyRateRepository : ICurrencyRateRepository
    {
        private readonly PicassoDbContext context;
        private readonly IObjectMapper mapper;

        public CurrencyRateRepository(PicassoDbContext context,IObjectMapper objectMapper)
        {
            this.context = context;
            this.mapper = objectMapper;
        }

        public async Task<IEnumerable<CurrencyRate>> GetCurencyRatesSince( DateTime sinceDay)
        {
            return await context.CurrencyRates.Where(cr =>  cr.Day >= sinceDay).ToListAsync();
        }

        public async Task<IEnumerable<(Currency, DateTime?)>> GetCurrenciesWithLastDownloadedDate()
        {
            var currencies = await context.Currencies.Where(c=> c.Symbol != "GBP").ToListAsync();
            var currencyRates = await context.CurrencyRates.GroupBy(cr => cr.From)
                .Select(x => new { From = x.Key, Day = (DateTime?)x.Max(p => p.Day) })
                .ToListAsync();

            var currencyWithDate = currencies.GroupJoin(currencyRates, c => c.Symbol, cr => cr.From, (c, cr) => (c, cr?.First()?.Day))
                .ToList();

            return currencyWithDate;

        }

        public async Task Save(IEnumerable<CurrencyRate> currencyRates)
        {
            foreach (var rate in currencyRates)
            {
                var existing = context.CurrencyRates.Where(cr => cr.From == rate.From && cr.Day == rate.Day).ToList();
                context.CurrencyRates.RemoveRange(existing);
            }

            context.CurrencyRates.AddRange(currencyRates);
            await context.SaveChangesAsync();

        }
    }
}
