using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.Service
{
    public interface ICurrencyService
    {
        Task Add(Currency symbol);

    }
    
}
