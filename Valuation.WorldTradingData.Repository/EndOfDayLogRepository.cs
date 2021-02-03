using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuation.Service;
using Valuation.Repository.Entities;

namespace Valuation.Repository
{
    public class EndOfDayLogRepository : IEndOfDayLogRepository
    {
        private readonly PicassoDbContext context;

        public EndOfDayLogRepository(PicassoDbContext context)
        {
            this.context = context;
        }

        public async Task Complete(int id)
        {
            var entity = context.EndOfDayPriceLogs.Find(id);
            entity.FinishedOn = DateTime.Now;
            entity.HasErrored = false;
            await context.SaveChangesAsync();
        }

        public async Task<bool> HasRunOn(DateTime day)
        {
            var entity = await context.EndOfDayPriceLogs.FirstOrDefaultAsync(eod => eod.Day == day.Date);
            if (entity != null && !entity.HasErrored)
                return true;
            return false;
        }

        public async Task SetErrored(int id)
        {
            var entity = context.EndOfDayPriceLogs.Find(id);
            entity.HasErrored = true;
            entity.FinishedOn = null;
            await context.SaveChangesAsync();
        }

        public async Task<int> Start()
        {
            var log = new EndOfDayPriceLogEntity { Day = DateTime.Now.Date, StartedOn = DateTime.Now };
            context.EndOfDayPriceLogs.Add(log);
            await context.SaveChangesAsync();
            return log.Id;
        }
    }
}
