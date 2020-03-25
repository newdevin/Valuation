using System;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public class CurrencyRatesLogService : ICurrencyRatesLogService
    {
        private readonly ICurrencyRatesLogRepository currencyRatesLogRepository;

        public CurrencyRatesLogService(ICurrencyRatesLogRepository currencyRatesLogRepository)
        {
            this.currencyRatesLogRepository = currencyRatesLogRepository;
        }
        public Task CurrencyRatesDownloadCompleted(int id)
        {
            return currencyRatesLogRepository.Complete(id);
        }

        public Task<bool> CurrencyRatesDownloadHasRunOn(DateTime day)
        {
            return currencyRatesLogRepository.HasRunOn(day);
        }

        public Task SetCurrencyRatesDownloadToErrored(int id)
        {
            return currencyRatesLogRepository.SetErrored(id);
        }

        public Task<int> CurrencyRatesDownloadStarted()
        {
            return currencyRatesLogRepository.Start();
        }
    }
}
