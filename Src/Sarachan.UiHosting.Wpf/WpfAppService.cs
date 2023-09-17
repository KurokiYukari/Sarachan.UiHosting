using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Sarachan.Ioc;
using Sarachan.UiHosting.Extensions;
using Sarachan.UiHosting.Wpf.Extensions;

namespace Sarachan.UiHosting.Wpf
{
    internal sealed class WpfAppService<TApp> : BackgroundService
        where TApp : Application
    {
        private readonly IServiceProvider _provider;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly IFallbackExceptionHandler _exceptionHandler;
        private readonly IOptionsMonitor<WpfUiOptions> _uiOptions;

        public WpfAppService(IServiceProvider provider,
            IHostApplicationLifetime applicationLifetime,
            IFallbackExceptionHandler exceptionHandler,
            IOptionsMonitor<WpfUiOptions> wpfUiOptions)
        {
            _provider = provider;
            _applicationLifetime = applicationLifetime;
            _exceptionHandler = exceptionHandler;
            _uiOptions = wpfUiOptions;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var options = _uiOptions.CurrentValue;

            using var serviceScope = _provider.CreateScope(services =>
            {
                services.AddSingleton<IUiContext, WpfUiContext>();
            });

            var provider = serviceScope.ServiceProvider;
            var uiContext = (WpfUiContext)provider.GetRequiredService<IUiContext>();

            var tcs = new TaskCompletionSource();
            var thread = ActivatorUtilities.CreateInstance<WpfAppThread<TApp>>(provider);
            var dispatcher = await thread.StartDispatcher(options.WpfAppThreadName);
            uiContext.Initialize(dispatcher);

            var defaultExceptionHandlerRegistration = _exceptionHandler.HandleDefaultExceptions();
            void ShutdownFinishedHandler(object? sender, EventArgs e)
            {
                tcs.SetResult();
                if (options.UseUiLifetime)
                {
                    _applicationLifetime.StopApplication();
                }

                defaultExceptionHandlerRegistration.Dispose();
            }

            if (dispatcher.HasShutdownFinished)
            {
                ShutdownFinishedHandler(dispatcher, EventArgs.Empty);
            }
            else
            {
                dispatcher.ShutdownFinished += ShutdownFinishedHandler;
            }

            stoppingToken.Register(() =>
            {
                dispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
            });

            await tcs.Task;
            return;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _exceptionHandler.HandleException(e.Exception);
            e.Handled = true;
        }
    }

    public interface IInitializeComponent
    {
        void InitializeComponent();
    }
}
