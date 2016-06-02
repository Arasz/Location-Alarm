using System;
using System.Diagnostics;

namespace LocationAlarm.Interceptors
{
    public static class AsyncErrorHandler
    {
        public static void HandleException(Exception exception)
        {
            Debug.WriteLine(exception);

            var innerException = exception.InnerException;
            while (innerException != null)
            {
                Debug.WriteLine(innerException);
                innerException = innerException.InnerException;
            }
        }
    }
}