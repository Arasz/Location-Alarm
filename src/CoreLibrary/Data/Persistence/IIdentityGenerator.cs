using CoreLibrary.DataModel;

namespace CoreLibrary.Data.Persistence
{
    /// <summary>
    /// Can assign unique identity to entity 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IIdentityGenerator
    {
        /// <summary>
        /// Assigns unique identity 
        /// </summary>
        void AssignUniqueIdentity<TEntity>(TEntity entity) where TEntity : IEntity;
    }
}