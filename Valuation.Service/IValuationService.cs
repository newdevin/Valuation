using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public interface IValuationService
    {
        Task ValuePortfolio(DateTime day);
    }

    //public class ValuationService : IValuationService
    //{
    //    public Task ValuePortfolio(DateTime day)
    //    {
            
    //    }
    //}

}

