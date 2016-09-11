using System.Collections.Generic;

namespace CoreLibrary.Data.Persistence.Mapping
{
    public class Configuration<TSource, TDestination> : IConfiguration<TSource, TDestination>
    {
        public IList<IMapper<TSource, TDestination>> Mappers { get; } = new List<IMapper<TSource, TDestination>>();

        public Configuration()
        {
            Mappers.Add(new SimpleReflectionMapper<TSource, TDestination>());
        }

        public IConfiguration<TSource, TDestination> AddMapping(IMapper<TSource, TDestination> mapper)
        {
            Mappers.Add(mapper);
            return this;
        }
    }
}