using System;

namespace CoreLibrary.Data.Persistence.Mapping
{
    internal class Mapper<TSource, TDestination> : IMapper<TSource, TDestination>
    {
        private readonly IConfiguration<TSource, TDestination> _configuration;

        public Mapper(IConfiguration<TSource, TDestination> configuration)
        {
            _configuration = configuration;
        }

        public TDestination Map(TSource source, TDestination destination = default(TDestination))
        {
            destination = Activator.CreateInstance<TDestination>();
            foreach (var mapper in _configuration.Mappers)
                mapper.Map(source, destination);

            return destination;
        }
    }
}