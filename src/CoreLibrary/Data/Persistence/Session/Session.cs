using CoreLibrary.Data.DataModel.Base;
using CoreLibrary.Data.Persistence.DataContext;
using System.Threading.Tasks;

namespace CoreLibrary.Data.Persistence.Session
{
    public class Session<TEntity> : SessionBase<TEntity>
        where TEntity : class, IEntity
    {
        public Session(IDataContext<TEntity> dataContext) : base(dataContext)
        {
        }

        public override Task FlushAsync()
        {
            throw new System.NotImplementedException();
        }

        public override Task OpenAsync()
        {
            throw new System.NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            throw new System.NotImplementedException();
        }
    }
}