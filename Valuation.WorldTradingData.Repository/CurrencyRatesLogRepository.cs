using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Valuation.Service;
using Valuation.Repository.Entities;

namespace Valuation.Repository
{
    public class CurrencyRatesLogRepository : ICurrencyRatesLogRepository
    {
        private readonly PicassoDbContext context;

        public CurrencyRatesLogRepository(PicassoDbContext context)
        {
            this.context = context;
        }

        public async Task Complete(int id)
        {
            var entity = context.CurrencyRatesLogs.Find(id);
            entity.FinishedOn = DateTime.Now;
            entity.HasErrored = false;
            await context.SaveChangesAsync();
        }

        public async Task<bool> HasRunOn(DateTime day)
        {
            var entity = await context.CurrencyRatesLogs.SingleOrDefaultAsync(eod => eod.Day == day.Date);
            if (entity != null && !entity.HasErrored)
                return true;
            return false;
        }

        public async Task SetErrored(int id)
        {
            var entity = context.CurrencyRatesLogs.Find(id);
            entity.HasErrored = true;
            entity.FinishedOn = null;
            await context.SaveChangesAsync();
        }

        public async Task<int> Start()
        {
            var log = new CurrencyRatesLogEntity { Day = DateTime.Now.Date, StartedOn = DateTime.Now };
            context.CurrencyRatesLogs.Add(log);
            await context.SaveChangesAsync();
            return log.Id;
        }
    }
}
