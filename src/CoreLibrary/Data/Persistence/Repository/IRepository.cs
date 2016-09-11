using CoreLibrary.Data.DataModel.Base;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreLibrary.Data.Persistence.Repository
{
    /// <summary>
    /// Generic repository interface 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> : IDisposable
        where TEntity : IEntity, new()
    {
        AsyncTableQuery<TEntity> CustomQuery { get; }

        Task DeleteAllAsync();

        Task<int> DeleteAsync(TEntity entity);

        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<TEntity> GetAsync(int id);

        Task<int> InsertAllAsync(IEnumerable<TEntity> entities);

        Task<int> InsertAsync(TEntity entity);

        Task<int> UpdateAllAsync(IEnumerable<TEntity> entities);

        Task<int> UpdateAsync(TEntity entity);
    }
}