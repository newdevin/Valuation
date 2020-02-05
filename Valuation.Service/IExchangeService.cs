using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.Service
{
    public interface IExchangeService
    {
        Task Add(Exchange exchange);
    }
}
