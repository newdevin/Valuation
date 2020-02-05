using System;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.Service
{
    public class ExchangeService : IExchangeService
    {
        private readonly IExchangeRepository exchangeRepository;

        public ExchangeService(IExchangeRepository exchangeRepository)
        {
            this.exchangeRepository = exchangeRepository;
        }

        public async Task Add(Exchange exchange)
        {
            Exchange exch = await exchangeRepository.Get(exchange.Symbol);
            if (exch != null)
                throw new InvalidOperationException($"{exchange.Symbol} exists");
            await exchangeRepository.Add(exchange);

        }
    }
}
