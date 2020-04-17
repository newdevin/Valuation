using System;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public class ValuationLogService : IValuationLogService
    {
        private readonly IValuationLogRepository valuationLogRepository;

        public ValuationLogService(IValuationLogRepository  valuatinoLogRepository)
        {
            this.valuationLogRepository = valuatinoLogRepository;
        }
        public Task<bool> HasValuationServiceRunOn(DateTime day)
        {
            return valuationLogRepository.HasValuationServiceRunOn(day);
        }

        public Task ValuationServiceCompleted(int id)
        {
            return valuationLogRepository.ValuationServiceCompleted(id);
        }

        public Task<int> ValuationServiceStarted()
        {
            return valuationLogRepository.ValuationServiceStarted();
        }
    }

}
