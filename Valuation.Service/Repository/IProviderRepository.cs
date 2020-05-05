using System.Threading.Tasks;

namespace Valuation.Service.Repository
{
    public interface IProviderRepository
    {
        Task<Provider> GetEmailProvider(string name);
    }
}