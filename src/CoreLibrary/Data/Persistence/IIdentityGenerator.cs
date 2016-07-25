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
        void AssignUniqueIdentity(IEntity entity);

        /// <summary>
        /// Returns entity identity to identity pool 
        /// </summary>
        void RecycleIdentity(IEntity entity);

        /// <summary>
        /// Returns identity to identity pool 
        /// </summary>
        void RecycleIdentity(int id);
    }
}