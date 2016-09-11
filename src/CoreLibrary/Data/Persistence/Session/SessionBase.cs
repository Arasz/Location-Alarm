using CoreLibrary.Data.DataModel.Base;
using CoreLibrary.Data.Persistence.Controller;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreLibrary.Data.Persistence.Session
{
    public abstract class SessionBase<TEntity> : ISession<TEntity>
        where TEntity : class, IEntity
    {
        private readonly IPersistenceController _persistenceController;
        public bool IsOpened { get; private set; }

        protected SessionBase(IPersistenceController persistenceController)
        {
            _persistenceController = persistenceController;
        }

        public virtual int Create(TEntity entity) => _dataContext.Create(entity);

        public virtual void Delete(TEntity entity) => _dataContext.Delete(entity);

        public virtual void Delete(int id) => _dataContext.Delete(id);

        public virtual void DeleteAll() => _dataContext.DeleteAll();

        public virtual void Dispose() => Dispose(true);

        public abstract Task FlushAsync();

        public abstract Task OpenAsync();

        public virtual TEntity Read(int id) => _dataContext.Read(id);

        public virtual IEnumerable<TEntity> ReadAll() => _dataContext.ReadAll();

        public virtual void Update(TEntity entity) => _dataContext.Update(entity);

        public virtual void UpdateAll(IEnumerable<TEntity> entities) => _dataContext.UpdateAll(entities);

        protected abstract void Dispose(bool disposing);
    }
}