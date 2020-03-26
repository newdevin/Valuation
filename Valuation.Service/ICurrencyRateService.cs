using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.Service
{
    public interface ICurrencyRateService
    {
        Task<IEnumerable<(Currency,DateTime?)>> GetActiveCurrenciesWithLastDownloadedDate();
        Task Save(IEnumerable<CurrencyRate> currencyRates);
        Task<IEnumerable<CurrencyRate>> GetCurencyRatesSince( DateTime sinceDay);
    }
}
