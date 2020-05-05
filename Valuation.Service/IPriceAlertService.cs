using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public interface IPriceAlertService
    {
        Task CheckAndSendAlert();
    }
}
