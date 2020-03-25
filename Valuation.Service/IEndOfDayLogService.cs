using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public interface IEndOfDayLogService
    {
        Task<int> Start();
        Task Complete(int id);
        Task<bool> HasRunOn(DateTime day);
        Task SetErrored(int id);
    }
}
