using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Valuation.Infrastructure
{
    public class ObjectMapper : IObjectMapper
    {
        private readonly IMapper mapper;

        public ObjectMapper(IMapper mapper)
        {
            this.mapper = mapper;
        }
        public T MapTo<U, T>(U u)
        {
            return mapper.Map<T>(u);
        }

        //public IEnumerable<T> MapTo<U, T>(IEnumerable<U> u)
        //{
        //    return u.Select(x => MapTo(x));
        //}
    }
}
