using System.Collections.Generic;

namespace CoreLibrary.Data.Persistence
{
    /// <summary>
    /// Persistent data context 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IDataContext<out TEntity> : ISelfSerializable
    {
        /// <summary>
        /// </summary>
        IEnumerable<TEntity> Entities { get; }
    }
}