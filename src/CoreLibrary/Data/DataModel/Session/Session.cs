using CoreLibrary.Data.DataModel.Base;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace CoreLibrary.Data.DataModel.Session
{
    public class Session : ISession
    {
        private readonly string _connection;
        private readonly Stream _stream;
        private LiteDatabase _dataContext;
        private Dictionary<string, object> Cache = new Dictionary<string, object>();
        public bool Disposed { get; }

        public Session(Stream stream)
        {
            _stream = stream;
            _dataContext = new LiteDatabase(stream);
        }

        public Session(string connection)
        {
            _connection = connection;
            _dataContext = new LiteDatabase(_connection);
        }

        public int Create<TEntity>(TEntity entity)
            where TEntity : IEntity, new()
        {
            var collection = GetCollection<TEntity>();

            var bsonValue = collection.Insert(entity);

            return bsonValue;
        }

        public void Delete<TEntity>(TEntity entity)
            where TEntity : IEntity, new()
        {
            var collection = GetCollection<TEntity>();
            collection.Delete(entity.Id);
        }

        public void Delete<TEntity>(int id)
            where TEntity : IEntity, new()
        {
            var collection = GetCollection<TEntity>();
            collection.Delete(id);
        }

        public void DeleteAll<TEntity>() where TEntity : IEntity, new()
        {
            GetCollection<TEntity>().Delete(Query.All());
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public TEntity Read<TEntity>(int id)
            where TEntity : IEntity, new()
        {
            return GetCollection<TEntity>().FindOne(entity => entity.Id == id);
        }

        public IEnumerable<TEntity> Read<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : IEntity, new()
        {
            return GetCollection<TEntity>().Find(predicate);
        }

        public IEnumerable<TEntity> ReadAll<TEntity>() where TEntity : IEntity, new()
        {
            return GetCollection<TEntity>().FindAll();
        }

        public void Update<TEntity>(TEntity entity)
            where TEntity : IEntity, new()
        {
            GetCollection<TEntity>().Update(entity.Id, entity);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing) return;

            _dataContext.Dispose();
            Cache = null;
        }

        private LiteCollection<TEntity> GetCollection<TEntity>() where TEntity : new()
        {
            if (!Cache.ContainsKey(typeof(TEntity).Name))
                Cache[typeof(TEntity).Name] = _dataContext.GetCollection<TEntity>(typeof(TEntity).Name);

            return Cache[typeof(TEntity).Name] as LiteCollection<TEntity>;
        }
    }
}