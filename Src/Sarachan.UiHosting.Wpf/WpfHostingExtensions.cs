using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sarachan.Ioc;
using Sarachan.UiHosting.Extensions;

namespace Sarachan.UiHosting.Wpf.Extensions
{
    public static class WpfHostingExtensions
    {
        public static IServiceCollection AddWpf<TApp>(this IServiceCollection self, Action<IWpfBuilder> builderDelegate)
            where TApp : Application
        {
            self.AddHostedService<WpfAppService<TApp>>();
            self.AddMergedProvider();
            var builder = new WpfBuilder(self);
            builderDelegate(builder);
            return self;
        }

        public static IHostBuilder AddWpf<TApp>(this IHostBuilder self, Action<IWpfBuilder> builderDelegate)
            where TApp : Application
        {
            self.ConfigureServices((ctx, services) =>
            {
                services.AddWpf<TApp>(builderDelegate)
                    .Configure<WpfUiOptions>(ctx.Configuration.GetSection(nameof(WpfUiOptions)));
            });

            return self;
        }
    }
}
