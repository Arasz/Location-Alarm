using CoreLibrary.Data.DataModel.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreLibrary.Data.DataModel.CRUD
{
    public interface ICrud
    {
        int Create<TEntity>(TEntity entity)
            where TEntity : IEntity, new();

        void Delete<TEntity>(TEntity entity)
            where TEntity : IEntity, new();

        void Delete<TEntity>(int id)
            where TEntity : IEntity, new();

        void DeleteAll<TEntity>()
            where TEntity : IEntity, new();

        TEntity Read<TEntity>(int id)
            where TEntity : IEntity, new();

        IEnumerable<TEntity> Read<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : IEntity, new();

        IEnumerable<TEntity> ReadAll<TEntity>()
             where TEntity : IEntity, new();

        void Update<TEntity>(TEntity entity)
            where TEntity : IEntity, new();
    }
}