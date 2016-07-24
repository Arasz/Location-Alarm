using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreLibrary.Data.Persistence.Session
{
    public class Session<TEntity> : ISession<TEntity>
    {
        private readonly IDataContext<TEntity> _dataContext;

        public bool IsOpened { get; private set; }

        public Session(IDataContext<TEntity> dataContext)
        {
            _dataContext = dataContext;
        }

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

        public void Dispose() => Dispose(true);

        public async Task FlushAsync()
        {
            var serializer = _dataContext as ISelfSerializable;
            await serializer.SerializeAsync().ConfigureAwait(false);
        }

        public async Task Open()
        {
            IsOpened = true;
            var serializer = _dataContext as ISelfSerializable;
            await serializer.DeserializeAsync().ConfigureAwait(false);
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

        protected virtual async void Dispose(bool disposing)
        {
            if (disposing)
                await FlushAsync().ConfigureAwait(false);
        }
    }
}