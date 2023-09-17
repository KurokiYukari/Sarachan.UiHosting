using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.HighPerformance.Buffers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sarachan.UiHosting.Extensions;
using Sarachan.UiHosting.GenericEditor.EditorConsole.Extensions;
using Sarachan.UiHosting.Wpf;
using Sarachan.UiHosting.Wpf.Extensions;

namespace Sarachan.UiHosting.WpfSample
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var services = new ServiceCollection();

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging((ctx, builder) =>
                {
                    builder.AddEditorConsole();
                })
                .ConfigureServices((ctx, services) =>
                {
                    services.AddUi(builder =>
                    {
                        builder.UseLoggingFallbackExceptionHandler();
                        builder.UseServiceProviderFactory(_ => new DefaultServiceProviderFactory());
                    });
                })
                .AddWpf<App>(builder => 
                {
                    
                })
                .Build();

            host.Run();
        }
    }
}
