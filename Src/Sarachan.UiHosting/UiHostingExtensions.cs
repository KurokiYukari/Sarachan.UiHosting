using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sarachan.UiHosting.Internals;

namespace Sarachan.UiHosting.Extensions
{
    public static class UiHostingExtensions
    {
        public static IUiBuilder UseLoggingFallbackExceptionHandler(this IUiBuilder self)
        {
            self.UseFallbackExceptionHandler(provider => ActivatorUtilities.CreateInstance<LoggingFallbackExceptionHandler>(provider));
            return self;
        }
    }
}
