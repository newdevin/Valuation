using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;
using Valuation.Service.Repository;

namespace Valuation.Repository
{
    public class CurrencyRepository : ICurrencyRepository
    {
        public Task Add(Currency symbol)
        {
            throw new NotImplementedException();
        }

        public Task<Currency> Get(string symbol)
        {
            throw new NotImplementedException();
        }
    }
}
