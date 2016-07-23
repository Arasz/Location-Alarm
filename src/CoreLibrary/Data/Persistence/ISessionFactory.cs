namespace CoreLibrary.Data.Persistence
{
    /// <summary>
    /// Responsible for creating and maintaining session objects 
    /// </summary>
    public interface ISessionFactory
    {
        ISession OpenSession();
    }
}