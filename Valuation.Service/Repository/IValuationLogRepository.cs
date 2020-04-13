using System;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public interface IValuationLogRepository
    {
        Task<bool> HasValuationServiceRunOn(DateTime day);
        Task ValuationServiceCompleted(int id);
        Task<int> ValuationServiceStarted();
    }
}