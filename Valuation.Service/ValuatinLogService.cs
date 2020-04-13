using System;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public class ValuatinLogService : IValuationLogService
    {
        private readonly IValuationLogRepository valuationLogRepository;

        public ValuatinLogService(IValuationLogRepository  valuatinoLogRepository)
        {
            this.valuationLogRepository = valuatinoLogRepository;
        }
        public Task<bool> HasValuatinServiceRunOn(DateTime day)
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
