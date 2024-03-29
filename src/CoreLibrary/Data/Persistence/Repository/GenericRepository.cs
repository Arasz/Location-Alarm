﻿using CoreLibrary.Data.DataModel.Base;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Windows.Storage;

namespace CoreLibrary.Data.Persistence.Repository
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : IEntity, new()
    {
        protected SQLiteAsyncConnection _asyncConnection;

        protected SQLiteConnection _connection;

        public AsyncTableQuery<TEntity> CustomQuery => AsyncConnection.Table<TEntity>();

        /// <summary>
        /// Cached async connection to database 
        /// </summary>
        protected SQLiteAsyncConnection AsyncConnection => _asyncConnection ?? (_asyncConnection = new SQLiteAsyncConnection(DatabasePath));

        /// <summary>
        /// Cached connection to database 
        /// </summary>
        protected SQLiteConnection Connection => _connection ?? (_connection = new SQLiteConnection(DatabasePath));

        /// <summary>
        /// Path to database 
        /// </summary>
        protected string DatabasePath => Path.Combine(ApplicationData.Current.LocalFolder.Path, $"{typeof(TEntity).Name}.db");

        public GenericRepository()
        {
            CreateTable();
        }

        public int Delete(TEntity entity) => Connection.Delete(entity);

        public void DeleteAll() => Connection.DeleteAll<TEntity>();

        public async Task DeleteAllAsync() => await AsyncConnection
            .RunInTransactionAsync(connection => connection.DeleteAll<TEntity>())
            .ConfigureAwait(false);

        public async Task<int> DeleteAsync(TEntity entity) => await AsyncConnection.DeleteAsync(entity)
            .ConfigureAwait(false);

        public void Dispose()
        {
            _connection.Dispose();
            _asyncConnection = null;
            _connection = null;
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate) => Connection.Table<TEntity>().Where(predicate);

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
                    => await AsyncConnection.Table<TEntity>()
                .Where(predicate)
                .ToListAsync()
                .ConfigureAwait(false);

        public TEntity Get(int id) => Connection.Get<TEntity>(id);

        public IEnumerable<TEntity> GetAll() => Connection.Table<TEntity>().ToList();

        public async Task<IEnumerable<TEntity>> GetAllAsync() => await AsyncConnection.Table<TEntity>()
                    .ToListAsync()
                    .ConfigureAwait(false);

        public async Task<TEntity> GetAsync(int id) => await AsyncConnection.GetAsync<TEntity>(id)
                    .ConfigureAwait(false);

        public int Insert(TEntity entity) => Connection.Insert(entity);

        public int InsertAll(IEnumerable<TEntity> entities) => Connection.InsertAll(entities);

        public async Task<int> InsertAllAsync(IEnumerable<TEntity> entities) => await AsyncConnection.InsertAllAsync(entities)
                    .ConfigureAwait(false);

        public async Task<int> InsertAsync(TEntity entity) => await AsyncConnection.InsertAsync(entity)
            .ConfigureAwait(false);

        public int Update(TEntity entity) => Connection.Update(entity);

        public int UpdateAll(IEnumerable<TEntity> entities) => Connection.UpdateAll(entities);

        public async Task<int> UpdateAllAsync(IEnumerable<TEntity> entities) => await AsyncConnection.UpdateAllAsync(entities)
            .ConfigureAwait(false);

        public async Task<int> UpdateAsync(TEntity entity) => await AsyncConnection.UpdateAsync(entity)
            .ConfigureAwait(false);

        protected void CreateTable() => Connection.CreateTable<TEntity>();

        /// <summary>
        /// Creates table for <typeparamref name="TEntity"/> 
        /// </summary>
        protected async Task CreateTableAsync() => await AsyncConnection.CreateTableAsync<TEntity>()
            .ConfigureAwait(false);
    }
}