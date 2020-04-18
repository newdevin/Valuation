using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Valuation.Repository.Mapper
{
    public class ObjectMapper : IObjectMapper
    {
        private readonly IMapper mapper;

        public ObjectMapper(IMapper mapper)
        {
            this.mapper = mapper;
        }
        public T MapTo<T>(object obj)
        {
            return mapper.Map<T>(obj);
        }

        public IEnumerable<T> MapTo<T>(IEnumerable<object> u)
        {
            return mapper.Map<IEnumerable<T>>(u);
        }
    }
}
