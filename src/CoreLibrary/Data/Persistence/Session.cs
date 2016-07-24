using CoreLibrary.DataModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreLibrary.Data.Persistence
{
    public class Session<TEntity> : ISession<TEntity>
    {
        private readonly IDataContext<GeolocationAlarm> _dataContext;

        public bool IsOpened { get; private set; }

        public Session(IDataContext<GeolocationAlarm> dataContext)
        {
            _dataContext = dataContext as DataContext<GeolocationAlarm>;
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

        public async Task FlushAsync() => await _dataContext.SerializeAsync().ConfigureAwait(false);

        public async Task Open()
        {
            IsOpened = true;
            await _dataContext.DeserializeAsync().ConfigureAwait(false);
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