using System.Threading.Tasks;

namespace Valuation.Service
{
    public interface IProviderService
    {
        Task<Provider> GetEmailProvider(string name);
    }
}
