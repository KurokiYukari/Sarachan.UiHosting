using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sarachan.UiHosting.Windows;

namespace Sarachan.UiHosting.Extensions
{
    public static class UiHostingAbstractionsExtensions
    {
        public static IServiceCollection AddUi<TUiContext>(this IServiceCollection self) where TUiContext : class, IUiContext
        {
            return self.AddSingleton<IUiContext, TUiContext>();
        }

        public static IUiBuilder UseFallbackExceptionHandler(this IUiBuilder builder, Func<IServiceProvider, IFallbackExceptionHandler> handler)
        {
            builder.Services.AddSingleton(handler);
            return builder;
        }

        public static IUiBuilder UseWindowService(this IUiBuilder builder, Func<IServiceProvider, IWindowService> windowService)
        {
            builder.Services.AddSingleton(windowService);
            return builder;
        }

        public static IDisposable HandleDefaultExceptions(this IFallbackExceptionHandler self)
        {
            return new DefaultExceptionHandler(self);
        }

        sealed class DefaultExceptionHandler : IDisposable
        {
            private readonly IFallbackExceptionHandler _fallbackExceptionHandler;

            public DefaultExceptionHandler(IFallbackExceptionHandler handler) 
            {
                _fallbackExceptionHandler = handler;

                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            }

            private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
            {
                if (_fallbackExceptionHandler.HandleException(e.Exception))
                {
                    e.SetObserved();
                }
            }

            private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
            {
                if (e.ExceptionObject is Exception exception)
                {
                    _fallbackExceptionHandler.HandleException(exception);
                }
            }

            public void Dispose()
            {
                AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
                TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
            }
        }
    }
}