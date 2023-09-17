using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sarachan.Ioc;

namespace Sarachan.UiHosting.Wpf.Extensions
{
    class WpfUiContextFactory : IUiContextFactory
    {
        private readonly IServiceProvider _provider;
        private readonly IOptionsMonitor<WpfUiOptions> _uiOptions;

        public WpfUiContextFactory(IServiceProvider provider,
            IOptionsMonitor<WpfUiOptions> uiOptions)
        {
            _provider = provider;
            _uiOptions = uiOptions;
        }

        public async ValueTask<IUiContext> StartNew(UiContextStartNewArgs args)
        {
            var options = _uiOptions.CurrentValue;

            var serviceScope = _provider.CreateScope(services =>
            {
                services.AddSingleton<IUiContext, WpfUiContext>();
            });

            var provider = serviceScope.ServiceProvider;
            var uiContext = (WpfUiContext)provider.GetRequiredService<IUiContext>();

            var thread = ActivatorUtilities.CreateInstance<WpfDispatcherThread>(provider);

            args.TryGetContext<string>(options.WpfThreadNameKey, out var threadName);

            var dispatcher = await thread.StartDispatcher(threadName ?? "WpfThread");
            dispatcher.ShutdownFinished += (sender, e) =>
            {
                serviceScope.Dispose();
            };

            uiContext.Initialize(dispatcher);
            return uiContext;
        }
    }
}
