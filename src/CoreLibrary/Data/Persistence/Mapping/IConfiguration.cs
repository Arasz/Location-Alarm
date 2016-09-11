using System.Collections.Generic;

namespace CoreLibrary.Data.Persistence.Mapping
{
    /// <summary>
    /// Mapping configuration 
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    public interface IConfiguration<TSource, TDestination>
    {
        IList<IMapper<TSource, TDestination>> Mappers { get; }

        IConfiguration<TSource, TDestination> AddMapping(IMapper<TSource, TDestination> mapper);
    }
}