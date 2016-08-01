using CoreLibrary.Data.DataModel.CRUD;
using System;

namespace CoreLibrary.Data.DataModel.Session
{
    public interface ISession : IDisposable, ICrud
    {
        bool Disposed { get; }
    }
}