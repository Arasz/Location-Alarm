using CoreLibrary.Data.Persistence.Session;
using CoreLibrary.DataModel;
using System.Threading.Tasks;

namespace CoreLibrary.Data.Persistence
{
    public class SessionFactory : ISessionFactory
    {
        private readonly IDataContext<GeolocationAlarm> _dataContext;

        public SessionFactory(IDataContext<GeolocationAlarm> dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ISession<TEntity>> OpenSessionAsync<TEntity>()
        {
            var session = new Session<TEntity>(_dataContext);
            await session.Open().ConfigureAwait(false);
            return session;
        }
    }
}