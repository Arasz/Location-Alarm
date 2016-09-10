using CoreLibrary.Data.DataModel.Base;
using CoreLibrary.Data.Persistence.DataContext;
using CoreLibrary.Data.Persistence.Serializer;

namespace CoreLibrary.Data.Persistence.Controller
{
    /// <summary>
    /// Responsible for data context persistence 
    /// </summary>
    internal class PersistenceController
    {
        public IDataContext<IEntity> DataContext { get; set; }

        public ISerialize<IEntity> DataContextSerializer { get; set; }

        public PersistenceController(IDataContext<IEntity> dataContext, ISerialize<IEntity> dataContextSerializer)
        {
            DataContext = dataContext;
            DataContextSerializer = dataContextSerializer;
        }
    }
}