using System.Collections.Generic;

namespace Valuation.Repository.Mapper
{
    public interface IObjectMapper
    {
        T MapTo<T>(object obj);
        IEnumerable<T> MapTo<T>(IEnumerable<object> u);
    }
}
