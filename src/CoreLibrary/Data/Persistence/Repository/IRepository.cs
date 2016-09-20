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

        int Delete(TEntity entity);

        void DeleteAll();

        Task DeleteAllAsync();

        Task<int> DeleteAsync(TEntity entity);

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity Get(int id);

        IEnumerable<TEntity> GetAll();

        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<TEntity> GetAsync(int id);

        int Insert(TEntity entity);

        int InsertAll(IEnumerable<TEntity> entities);

        Task<int> InsertAllAsync(IEnumerable<TEntity> entities);

        Task<int> InsertAsync(TEntity entity);

        int Update(TEntity entity);

        int UpdateAll(IEnumerable<TEntity> entities);

        Task<int> UpdateAllAsync(IEnumerable<TEntity> entities);

        Task<int> UpdateAsync(TEntity entity);
    }
}