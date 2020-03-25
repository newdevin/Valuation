using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.Service
{
    public interface ICurrencyRateRepository
    {
        Task<IEnumerable<(Currency, DateTime?)>>GetCurrenciesWithLastDownloadedDate();
        Task Save(IEnumerable<CurrencyRate> currencyRates);
    }
}