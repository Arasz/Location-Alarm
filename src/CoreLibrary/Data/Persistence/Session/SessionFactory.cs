using CoreLibrary.Data.Persistence.Session;
using CoreLibrary.DataModel;
using System.Threading.Tasks;

namespace CoreLibrary.Data.Persistence
{
    public class SessionFactory<TEntity> : ISessionFactory<TEntity>
                    where TEntity : class, IEntity
    {
        private readonly IDataContext<TEntity> _dataContext;

        public SessionFactory(IDataContext<TEntity> dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ISession<TEntity>> OpenSessionAsync()

        {
            var session = new Session<TEntity>(_dataContext);
            await session.OpenAsync().ConfigureAwait(false);
            return session;
        }
    }
}