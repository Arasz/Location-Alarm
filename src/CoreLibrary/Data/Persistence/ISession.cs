using System;
using System.Threading.Tasks;

namespace CoreLibrary.Data.Persistence
{
    /// <summary>
    /// Represents batch of operations on data storage 
    /// </summary>
    public interface ISession<TEntity> : ICrudable<TEntity>, IDisposable
    {
        bool IsOpened { get; }

        Task FlushAsync();
    }
}