using System;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public class EndOfDayLogService : IEndOfDayLogService
    {
        private readonly IEndOfDayLogRepository endOfDayLogRepository;

        public EndOfDayLogService(IEndOfDayLogRepository endOfDayLogRepository)
        {
            this.endOfDayLogRepository = endOfDayLogRepository;
        }
        public Task EndOfDayPriceDownloadCompleted(int id)
        {
            return endOfDayLogRepository.Complete(id);
        }

        public Task<bool> EndOfDayPriceDownloadHasRunOn(DateTime day)
        {
            return endOfDayLogRepository.HasRunOn(day);
        }

        public Task SetEndOfDayDownloadToErrored(int id)
        {
            return endOfDayLogRepository.SetErrored(id);
        }

        public Task<int> EndOfDayPriceDownloadStarted()
        {
            return endOfDayLogRepository.Start();
        }
    }
}
