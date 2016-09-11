using CoreLibrary.Data.DataModel.Base;
using CoreLibrary.Data.Persistence.Controller;
using CoreLibrary.Data.Persistence.Session;
using System.Threading.Tasks;

namespace CoreLibrary.Data.Persistence
{
    public class SessionFactory<TEntity> : ISessionFactory<TEntity>
                    where TEntity : class, IEntity
    {
        private readonly IPersistenceController _persistenceController;

        public SessionFactory(IPersistenceController persistenceController)
        {
            _persistenceController = persistenceController;
        }

        public async Task<ISession<TEntity>> OpenSessionAsync()

        {
            var session = new Session<TEntity>(_persistenceController);
            await session.OpenAsync().ConfigureAwait(false);
            return session;
        }
    }
}