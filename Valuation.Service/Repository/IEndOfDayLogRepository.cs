using System;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public interface IEndOfDayLogRepository
    {
        public Task<int> Start();

        public Task Complete(int id);

        public Task<bool> HasRunOn(DateTime day);
        Task SetErrored(int id);
    }
}