using System;
using System.Threading.Tasks;

namespace CoreLibrary.Logger
{
    public interface ILogger
    {
        void LogError(string message);

        Task LogErrorAsync(string message);

        void LogException(string message, Exception exception);

        Task LogExceptionAsync(string message, Exception exception);

        void LogInfo(string message);

        Task LogInfoAsync(string message);
    }
}