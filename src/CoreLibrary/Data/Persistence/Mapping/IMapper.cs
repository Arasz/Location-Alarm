namespace CoreLibrary.Data.Persistence.Mapping
{
    public interface IMapper<in TSource, TDestination>
    {
        TDestination Map(TSource source, TDestination destination = default(TDestination));
    }
}