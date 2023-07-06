using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Sarachan.UiHosting.Extensions;
using Sarachan.UiHosting.Wpf.Extensions;

namespace Sarachan.UiHosting.Wpf
{
    internal sealed class WpfService<TApp> : BackgroundService
        where TApp : Application
    {
        private const string NAME = $"WpfService";

        private readonly IUiContext _uiContext;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly IFallbackExceptionHandler _exceptionHandler;
        private readonly IOptionsMonitor<WpfUiOptions> _uiOptions;

        public TApp? App { get; private set; }

        public WpfService(IUiContext uiContext, 
            IHostApplicationLifetime applicationLifetime,
            IFallbackExceptionHandler exceptionHandler,
            IOptionsMonitor<WpfUiOptions> wpfUiOptions)
        {
            _uiContext = uiContext;
            _applicationLifetime = applicationLifetime;
            _exceptionHandler = exceptionHandler;
            _uiOptions = wpfUiOptions;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var defaultExceptionHandlerRegistration = _exceptionHandler.HandleDefaultExceptions();

            var tcs = new TaskCompletionSource();

            var uiThread = new Thread(() =>
            {
                Debug.Assert(App == null);

                try
                {
                    var app = App = ActivatorUtilities.GetServiceOrCreateInstance<TApp>(_uiContext.Provider);
                    app.DispatcherUnhandledException += App_DispatcherUnhandledException;
                    if (app is IInitializeComponent initializeComponent)
                    {
                        initializeComponent.InitializeComponent();
                    }

                    stoppingToken.Register(() =>
                    {
                        var app = App;
                        app?.Dispatcher.BeginInvoke(static (Application state) =>
                        {
                            state.Shutdown();
                        }, app);
                        // _uiTaskCompleteSource.SetCancel
                    });

                    app.Run();
                }
                finally
                {
                    App = null;
                    tcs.SetResult();

                    if (_uiOptions.CurrentValue.UseUiLifetime)
                    {
                        _applicationLifetime.StopApplication();
                    }

                    defaultExceptionHandlerRegistration.Dispose();
                }
            })
            {
                IsBackground = true,
                Name = NAME,
            };
            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();

            return tcs.Task;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            _exceptionHandler.HandleException(e.Exception);
            e.Handled = true;
        }
    }

    public interface IInitializeComponent
    {
        void InitializeComponent();
    }

    internal sealed class WpfContext : IUiContext
    {
        public IServiceProvider Provider { get; }

        public WpfContext(IServiceProvider provider)
        {
            Provider = provider;
        }

        public void Invoke(Action<object> action, object state)
        {
            var app = Application.Current;
            if (app == null)
            {
                action(state);
            }
            else
            {
                app.Dispatcher.BeginInvoke(action, state);
            }
        }
    }
}
