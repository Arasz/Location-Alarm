using System;
using System.Threading.Tasks;

namespace CoreLibrary.Logger
{
    public interface ILogger
    {
        Task LogErrorAsync(string message);

        Task LogExceptionAsync(string message, Exception exception);

        Task LogInfoAsync(string message);
    }
}