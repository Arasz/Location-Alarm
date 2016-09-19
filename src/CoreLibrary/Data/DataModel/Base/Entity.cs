using SQLite;

namespace CoreLibrary.Data.DataModel.Base
{
    /// <summary>
    /// Base class for all persistent objects 
    /// </summary>
    [Equals]
    public class Entity : IEntity
    {
        /// <summary>
        /// Entity id 
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public virtual IEntity Clone()
        {
            return new Entity
            {
                Id = Id,
            };
        }
    }
}