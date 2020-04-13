using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuation.Repository.Entities;
using Valuation.Service;

namespace Valuation.Repository
{
    public class ValuationLogRepository : IValuationLogRepository
    {
        private readonly PicassoDbContext context;

        public ValuationLogRepository(PicassoDbContext context)
        {
            this.context = context;
        }
        public async Task<bool> HasValuationServiceRunOn(DateTime day)
        {
            var log = await context.ValuationLogs.FirstOrDefaultAsync(vl => vl.Day.Date == day.Date);
            if (log == null)
                return false;
            return true;
        }

        public async Task ValuationServiceCompleted(int id)
        {
            var log = await context.ValuationLogs.FindAsync(id);
            log.FinishedOn = DateTime.Now;
            await context.SaveChangesAsync();
        }

        public async Task<int> ValuationServiceStarted()
        {
            var log = new ValuationLogEntity { Day = DateTime.Now.Date, StartedOn = DateTime.Now };
            context.ValuationLogs.Add(log);
            await context.SaveChangesAsync();
            return log.Id;
        }
    }
}
