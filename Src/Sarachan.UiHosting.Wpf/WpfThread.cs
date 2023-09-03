using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Sarachan.UiHosting.Wpf
{
    public abstract class WpfThread
    {
        private readonly IFallbackExceptionHandler _exceptionHandler;

        private readonly TaskCompletionSource<Dispatcher> _tcs = new();

        private readonly Lazy<Thread> _thread;

        public WpfThread(IFallbackExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;

            _thread = new Lazy<Thread>(() =>
            {
                var thread = new Thread(ThreadStart);
                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = true;
                return thread;
            }, true);
        }

        private void ThreadStart()
        {
            StartDispatcher(_tcs);
        }

        protected abstract void StartDispatcher(TaskCompletionSource<Dispatcher> tcs);

        public async Task<Dispatcher> StartDispatcher(string? name)
        {
            var thread = _thread.Value;
            thread.Name = name;
            thread.Start();
            var dispatcher = await _tcs.Task;
            dispatcher.UnhandledException += Dispatcher_UnhandledException;
            return dispatcher;
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = _exceptionHandler.HandleException(e.Exception);
        }
    }

    public sealed class WpfAppThread<TApp> : WpfThread 
        where TApp : Application
    {
        private readonly IServiceProvider _provider;

        public WpfAppThread(IServiceProvider provider, IFallbackExceptionHandler exceptionHandler) 
            : base(exceptionHandler)
        {
            _provider = provider;
        }

        protected override void StartDispatcher(TaskCompletionSource<Dispatcher> tcs)
        {
            var dispatcher = Dispatcher.CurrentDispatcher;
            //var uiContext = new WpfContext(_provider, dispatcher);
            var app = ActivatorUtilities.CreateInstance<TApp>(_provider);
            if (app is IInitializeComponent initializeComponent)
            {
                initializeComponent.InitializeComponent();
            }

            tcs.SetResult(app.Dispatcher);
            app.Run();
        }
    }

    public sealed class WpfDispatcherThread : WpfThread
    {
        public WpfDispatcherThread(IFallbackExceptionHandler exceptionHandler) : base(exceptionHandler)
        {
        }

        protected override void StartDispatcher(TaskCompletionSource<Dispatcher> tcs)
        {
            var dispatcher = Dispatcher.CurrentDispatcher;
            tcs.SetResult(dispatcher);
            Dispatcher.Run();
        }
    }
}
