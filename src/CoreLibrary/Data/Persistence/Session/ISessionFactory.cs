﻿using CoreLibrary.Data.DataModel.Base;
using CoreLibrary.Data.Persistence.Session;
using System.Threading.Tasks;

namespace CoreLibrary.Data.Persistence
{
    /// <summary>
    /// Responsible for creating and maintaining session objects 
    /// </summary>
    public interface ISessionFactory<TEntity>
                    where TEntity : class, IEntity
    {
        Task<ISession<TEntity>> OpenSessionAsync();
    }
}