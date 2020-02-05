using System;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.Service
{
    public interface IExchangeRepository
    {
        Task<Exchange> Get(string symbol);
        Task Add(Exchange exchange);
    }
}