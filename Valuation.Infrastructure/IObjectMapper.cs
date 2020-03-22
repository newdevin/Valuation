using System.Collections.Generic;

namespace Valuation.Infrastructure
{
    public interface IObjectMapper
    {
        T MapTo<U, T>(U u);
      //  IEnumerable<T> MapTo<U, T>(IEnumerable<U> u);
    }
}
