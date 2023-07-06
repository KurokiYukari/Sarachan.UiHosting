using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Sarachan.UiHosting.Internals
{
    public class LoggingFallbackExceptionHandler : IFallbackExceptionHandler
    {
        private readonly ILogger _logger;

        public LoggingFallbackExceptionHandler(ILogger<LoggingFallbackExceptionHandler> logger)
        {
            _logger = logger;
        }

        public bool HandleException(Exception exception)
        {
            _logger.LogError(exception, "{message}", exception.Message);
            return true;
        }
    }
}
