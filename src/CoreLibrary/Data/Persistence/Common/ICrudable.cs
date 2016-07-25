using CoreLibrary.DataModel;
using System.Collections.Generic;

namespace CoreLibrary.Data.Persistence.Common
{
    /// <summary>
    /// Defines CRUD operations 
    /// </summary>
    public interface ICrudable<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Creates entity in persistent storage 
        /// </summary>
        /// <returns> Id of entity </returns>
        int Create(TEntity entity);

        /// <summary>
        /// Deletes entity from persistent storage 
        /// </summary>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes entity from persistent storage 
        /// </summary>
        void Delete(int id);

        /// <summary>
        /// Deletes all entities 
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// Reads entity from persistent storage 
        /// </summary>
        TEntity Read(int id);

        /// <summary>
        /// Read all entities from persisten storage 
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> ReadAll();

        /// <summary>
        /// Updates entity in persistent storage 
        /// </summary>
        /// <param name="entity"></param>
        void Update(TEntity entity);

        /// <summary>
        /// Updates all entities from given collection 
        /// </summary>
        /// <param name="entities"></param>
        void UpdateAll(IEnumerable<TEntity> entities);
    }
}