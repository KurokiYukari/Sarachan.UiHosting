using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Extensions.Options;
using Sarachan.UiHosting.Wpf.Extensions;

namespace Sarachan.UiHosting.Wpf
{
    class WpfUiContext : IUiContext
    {
        public IServiceProvider Provider { get; }

        private readonly IOptionsMonitor<WpfUiOptions> _wpfUiOptions;

        private Dispatcher? _dispatcher;
        public Dispatcher Dispatcher => _dispatcher ?? throw new InvalidOperationException("WpfUiContext has not been initialized");

        public WpfUiContext(IServiceProvider provider,
            IOptionsMonitor<WpfUiOptions> wpfUiOptions)
        {
            Provider = provider;
            _wpfUiOptions = wpfUiOptions;
        }

        public void Initialize(Dispatcher dispatcher)
        {
            if (_dispatcher != null)
            {
                throw new InvalidOperationException("WpfUiContext has been initialized");
            }

            _dispatcher = dispatcher;
        }

        public Task<TResult> InvokeAsync<TResult>(Func<UiContextInvokeArgs?, TResult> function, UiContextInvokeArgs? args)
        {
            var options = _wpfUiOptions.CurrentValue;

            var priorityKey = options.ContextDispatcherPriorityKey;

            var priority = DispatcherPriority.Normal;
            var token = CancellationToken.None;
            if (args != null)
            {
                if (args.TryGetContext<object>(priorityKey, out var priorityContext))
                {
                    if (priorityContext is string str)
                    {
                        priority = Enum.Parse<DispatcherPriority>(str, true);
                    }
                    else
                    {
                        int priorityValue = (int)Convert.ChangeType(priorityContext, typeof(int))!;
                        priority = (DispatcherPriority)priorityValue;
                    }
                }

                token = args.CancellationToken;
            }

            var operation = Dispatcher.InvokeAsync(() => function(args), priority, token);
            return operation.Task;
        }

        public void Dispose()
        {
            _dispatcher?.InvokeShutdown();
        }
    }
}
