using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.Service.Repository
{
    public interface ICurrencyRepository
    {
        Task Add(Currency symbol);
        Task<Currency> Get(string symbol);
    }
}
