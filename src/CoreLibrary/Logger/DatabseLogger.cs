using CoreLibrary.Data.Persistence.Repository;
using System;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Logger
{
    public class DatabseLogger : ILogger
    {
        private IRepository<Log> _loggingRepository;

        public DatabseLogger(IRepository<Log> loggingRepository)
        {
            _loggingRepository = loggingRepository;
        }

        public async Task LogErrorAsync(string message)
        {
            await _loggingRepository.InsertAsync(new Log
            {
                Level = nameof(LoggingLevels.Error),
                Message = message,
                StackTrace = "",
                Type = nameof(LoggingLevels.Error),
                Time = DateTime.Now,
            }).ConfigureAwait(false);
        }

        public async Task LogExceptionAsync(string message, Exception exception)
        {
            await _loggingRepository.InsertAsync(new Log
            {
                Level = nameof(LoggingLevels.Error),
                Message = message,
                ExceptionMessage = exception.Message,
                StackTrace = exception.StackTrace,
                Type = nameof(Exception),
                Exceptions = ParseExceptions(exception),
                Time = DateTime.Now,
            }).ConfigureAwait(false);
        }

        public async Task LogInfoAsync(string message)
        {
            await _loggingRepository.InsertAsync(new Log
            {
                Level = nameof(LoggingLevels.Info),
                Message = message,
                StackTrace = "",
                Type = nameof(LoggingLevels.Info),
                Time = DateTime.Now,
            }).ConfigureAwait(false);
        }

        private string ParseExceptions(Exception exception)
        {
            StringBuilder aggregatedException = new StringBuilder();
            var innerException = exception;
            while (innerException != null)
            {
                aggregatedException.AppendLine(innerException.ToString());
                aggregatedException.AppendLine();
                innerException = innerException.InnerException;
            }
            return aggregatedException.ToString();
        }
    }
}