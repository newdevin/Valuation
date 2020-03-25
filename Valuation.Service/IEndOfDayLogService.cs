using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public interface IEndOfDayLogService
    {
        Task<int> EndOfDayPriceDownloadStarted();
        Task EndOfDayPriceDownloadCompleted(int id);
        Task<bool> EndOfDayPriceDownloadHasRunOn(DateTime day);
        Task SetEndOfDayDownloadToErrored(int id);
    }

}
