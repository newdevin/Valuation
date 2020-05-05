using System.Threading.Tasks;
using Valuation.Service.Repository;

namespace Valuation.Service
{
    public class ProviderService : IProviderService
    {
        private readonly IProviderRepository providerRepository;

        public ProviderService(IProviderRepository providerRepository)
        {
            this.providerRepository = providerRepository;
        }
        public Task<Provider> GetEmailProvider(string name)
        {
            return providerRepository.GetEmailProvider(name);
        }
    }
}
