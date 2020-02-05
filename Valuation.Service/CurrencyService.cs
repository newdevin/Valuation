using System;
using System.Threading.Tasks;
using Valuation.Domain;
using Valuation.Service.Repository;

namespace Valuation.Service
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository currencyRepositiory;

        public CurrencyService(ICurrencyRepository currencyRepositiory)
        {
            this.currencyRepositiory = currencyRepositiory;
        }
        public async Task Add(Currency currency)
        {
            var curr = await currencyRepositiory.Get(currency.Symbol);
            if (curr != null)
                throw new InvalidOperationException($"{currency.Symbol} already exists");
            await currencyRepositiory.Add(currency);
        }
    }

}
