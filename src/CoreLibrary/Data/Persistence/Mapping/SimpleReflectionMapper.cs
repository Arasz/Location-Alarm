using System;
using System.Linq;
using System.Reflection;

namespace CoreLibrary.Data.Persistence.Mapping
{
    /// <summary>
    /// Maps properties with the same name from source type to destination type 
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    internal class SimpleReflectionMapper<TSource, TDestination> : IMapper<TSource, TDestination>
    {
        public TDestination Map(TSource source, TDestination destination)
        {
            var sourceProperties = typeof(TSource).GetRuntimeProperties().Where(info => info.CanWrite).ToList();

            var destinationProperties = typeof(TDestination).GetRuntimeProperties().Where(info => info.CanWrite).ToList();

            foreach (var sourceProperty in sourceProperties)
            {
                var destinationProperty = destinationProperties.FirstOrDefault(info => string.Equals(info.Name, sourceProperty.Name, StringComparison.OrdinalIgnoreCase));

                destinationProperty?.SetValue(destination, sourceProperty.GetValue(source));
            }

            return destination;
        }
    }
}