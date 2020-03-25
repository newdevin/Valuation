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
        public Task Complete(int id)
        {
            return endOfDayLogRepository.Complete(id);
        }

        public Task<bool> HasRunOn(DateTime day)
        {
            return endOfDayLogRepository.HasRunOn(day);
        }

        public Task SetErrored(int id)
        {
            return endOfDayLogRepository.SetErrored(id);
        }

        public Task<int> Start()
        {
            return endOfDayLogRepository.Start();
        }
    }
}
