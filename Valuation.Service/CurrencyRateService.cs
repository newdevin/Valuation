using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.Service
{
    public class CurrencyRateService : ICurrencyRateService
    {
        private readonly ICurrencyRateRepository currencyRateRepository;

        public CurrencyRateService(ICurrencyRateRepository currencyRateRepository)
        {
            this.currencyRateRepository = currencyRateRepository;
        }
        public Task<IEnumerable<(Currency, DateTime?)>> GetActiveCurrenciesWithLastDownloadedDate()
        {
            return currencyRateRepository.GetCurrenciesWithLastDownloadedDate();
        }

        public Task<IEnumerable<CurrencyRate>> GetCurencyRatesSince( DateTime sinceDay)
        {
            return currencyRateRepository.GetCurencyRatesSince( sinceDay);
        }

        public Task Save(IEnumerable<CurrencyRate> currencyRates)
        {
            return currencyRateRepository.Save(currencyRates);
        }
    }
}
