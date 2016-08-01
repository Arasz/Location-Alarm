namespace CoreLibrary.Data.DataModel.Session
{
    /// <summary>
    /// Creates and manages session objects 
    /// </summary>
    public interface ISessionFactory
    {
        ISession CurrentSession { get; }

        void Reset();
    }
}