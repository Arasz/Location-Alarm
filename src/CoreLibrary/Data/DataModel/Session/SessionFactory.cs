using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace CoreLibrary.Data.DataModel.Session
{
    public class SessionFactory : ISessionFactory
    {
        private ISession _currentSession;

        public ISession CurrentSession
        {
            get
            {
                if (_currentSession == null || _currentSession.Disposed)
                    _currentSession = new Session(Connection);
                return _currentSession;
            }
        }

        private string Connection { get; }

        public SessionFactory(string connection = "data.db")
        {
            Connection = Path.Combine(ApplicationData.Current.LocalFolder.Path, connection);
            CreateDbFileAsync(connection).Wait();
        }

        public async Task CreateDbFileAsync(string connection)
        {
            var localFolder = ApplicationData.Current.LocalFolder;

            try
            {
                var file = await localFolder.GetFileAsync(connection);
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                await localFolder.CreateFileAsync(connection);
            }
        }

        public void Reset() => _currentSession.Dispose();
    }
}