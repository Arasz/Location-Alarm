using CoreLibrary.DataModel;
using System;
using System.Threading.Tasks;

namespace CoreLibrary.Data.Persistence.Session
{
    /// <summary>
    /// Represents batch of operations on data storage 
    /// </summary>
    public interface ISession<TEntity> : ICrudable<TEntity>, IDisposable
        where TEntity : class, IEntity
    {
        bool IsOpened { get; }

        Task FlushAsync();
    }
}