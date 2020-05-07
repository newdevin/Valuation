using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.Service
{
    public interface IQuoteService
    {
        Task<IEnumerable<Quote>> GetQuotes(IEnumerable<Listing> listings);
    }
}
