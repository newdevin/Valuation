using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Valuation.WorldTradingData.Service
{
    public interface IApiRepository
    {
        Task<List<string>> GetTokens(string name);
    }
}
