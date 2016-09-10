using CoreLibrary.Data.DataModel.Base;
using CoreLibrary.Data.Persistence.DataContext;
using System.Threading.Tasks;

namespace CoreLibrary.Data.Persistence.Serializer
{
    /// <summary>
    /// Serializes data context 
    /// </summary>
    public interface ISerialize<TSerializable>
        where TSerializable : class, IEntity
    {
        Task<IDataContext<TSerializable>> DeserializeAsync();

        Task SerializeAsync(IDataContext<TSerializable> dataContext);
    }
}