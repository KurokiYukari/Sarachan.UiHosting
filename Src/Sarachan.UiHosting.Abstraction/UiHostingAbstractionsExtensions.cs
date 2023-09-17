using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sarachan.UiHosting.Navigation;

namespace Sarachan.UiHosting.Extensions
{
    public static class UiHostingAbstractionsExtensions
    {
        public static IServiceCollection AddUi(this IServiceCollection services, Action<IUiBuilder> builderAction)
        {
            var builder = new UiBuilder(services);
            builderAction(builder);
            return services;
        }

        public static IUiBuilder UseUiContextFactory(this IUiBuilder self, Func<IServiceProvider, IUiContextFactory> implementation)
        {
            self.Services.AddSingleton(implementation);
            return self;
        }

        public static IUiBuilder UseFallbackExceptionHandler(this IUiBuilder self, Func<IServiceProvider, IFallbackExceptionHandler> implementation)
        {
            self.Services.AddSingleton(implementation);
            return self;
        }

        public static IUiBuilder UseServiceProviderFactory(this IUiBuilder self, Func<IServiceProvider, IServiceProviderFactory<IServiceCollection>> implementation)
        {
            self.Services.AddSingleton(implementation);
            return self;
        }

        public static IUiBuilder UseNavigationService(this IUiBuilder self, Func<IServiceProvider, INavigationService> implementation)
        {
            self.Services.AddSingleton(implementation);
            return self;
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