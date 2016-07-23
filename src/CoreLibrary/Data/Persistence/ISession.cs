using System;

namespace CoreLibrary.Data.Persistence
{
    /// <summary>
    /// Represents batch of operations on data storage 
    /// </summary>
    public interface ISession : IDisposable
    {
        void Flush();
    }
}