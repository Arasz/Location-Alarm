using CoreLibrary.Data.Persistence;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CoreLibrary.Data
{
    /// <summary>
    /// Encapsulates persistent data 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    [DataContract]
    public class DataContext<TEntity> : IDataContext<TEntity>
    {
        [DataMember]
        private Dictionary<int, TEntity> _entities = new Dictionary<int, TEntity>();

        public IEnumerable<TEntity> Entities => _entities.Values;

        [DataMember]
        public IIdentityGenerator IdentityGenerator { get; set; }

        public int Create(TEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(TEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteAll()
        {
            throw new System.NotImplementedException();
        }

        public TEntity Read(int id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<TEntity> ReadAll()
        {
            throw new System.NotImplementedException();
        }

        public void Update(TEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateAll(IEnumerable<TEntity> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}