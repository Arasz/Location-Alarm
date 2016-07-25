using CoreLibrary.DataModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreLibrary.Data.Persistence.Session
{
    public class Session<TEntity> : ISession<TEntity>
        where TEntity : class, IEntity
    {
        private readonly IDataContext<TEntity> _dataContext;

        public bool IsOpened { get; private set; }

        public Session(IDataContext<TEntity> dataContext)
        {
            _dataContext = dataContext;
        }

        public int Create(TEntity entity) => _dataContext.Create(entity);

        public void Delete(TEntity entity) => _dataContext.Delete(entity);

        public void Delete(int id) => _dataContext.Delete(id);

        public void DeleteAll() => _dataContext.DeleteAll();

        public void Dispose() => Dispose(true);

        public async Task FlushAsync()
        {
            var serializer = _dataContext as ISelfSerializable;
            await serializer.SerializeAsync().ConfigureAwait(false);
        }

        public async Task OpenAsync()
        {
            IsOpened = true;
            var serializer = _dataContext as ISelfSerializable;
            await serializer.DeserializeAsync().ConfigureAwait(false);
        }

        public TEntity Read(int id) => _dataContext.Read(id);

        public IEnumerable<TEntity> ReadAll() => _dataContext.ReadAll();

        public void Update(TEntity entity) => _dataContext.Update(entity);

        public void UpdateAll(IEnumerable<TEntity> entities) => _dataContext.UpdateAll(entities);

        protected virtual async void Dispose(bool disposing)
        {
            if (disposing)
                await FlushAsync().ConfigureAwait(false);
        }
    }
}