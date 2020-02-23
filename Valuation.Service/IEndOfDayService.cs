using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public interface IEndOfDayService
    {
        Task DownloadEndOfDayPrices(DateTime endOfDay);
    }
        
}
