using System;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public interface ICurrencyRatesLogRepository
    {
        public Task<int> Start();

        public Task Complete(int id);

        public Task<bool> HasRunOn(DateTime day);
        Task SetErrored(int id);
    }
}