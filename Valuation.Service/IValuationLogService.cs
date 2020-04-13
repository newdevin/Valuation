using System;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public interface IValuationLogService
    {
        Task<int> ValuationServiceStarted();
        Task ValuationServiceCompleted(int id);
        Task<bool> HasValuationServiceRunOn(DateTime day);
    }

}
