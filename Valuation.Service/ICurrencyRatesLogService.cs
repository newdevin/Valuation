using System;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public interface ICurrencyRatesLogService
    {
        Task<int> CurrencyRatesDownloadStarted();
        Task CurrencyRatesDownloadCompleted(int id);

        Task<bool> CurrencyRatesDownloadHasRunOn(DateTime day);
        Task SetCurrencyRatesDownloadToErrored(int id);
    }

}
