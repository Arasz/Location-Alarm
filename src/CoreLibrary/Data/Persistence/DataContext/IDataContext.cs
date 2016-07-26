using CoreLibrary.Data.DataModel.Base;
using CoreLibrary.Data.Persistence.Common;
using System.Collections.Generic;

namespace CoreLibrary.Data.Persistence.DataContext
{
    /// <summary>
    /// Persistent data context 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IDataContext<TEntity> : ICrudable<TEntity>
        where TEntity : class, IEntity
    {
        IEnumerable<TEntity> Entities { get; }
    }
}