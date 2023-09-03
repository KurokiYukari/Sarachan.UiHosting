using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Sarachan.Ioc
{
    public static class MergedServiceProviderExtensions
    {
        public static IServiceCollection AddMergedProvider(this IServiceCollection self)
        {
            self.TryAddSingleton<IMergedServiceProviderFactory, MergedServiceProviderFactory>();
            return self;
        }

        public static IServiceScope CreateScope(this IServiceProvider self, Action<IServiceCollection> configure)
        {
            var services = new ServiceCollection();
            configure(services);

            return CreateScope(self, services);
        }

        public static IServiceScope CreateScope(this IServiceProvider self, IServiceCollection services)
        { 
            var factory = self.GetRequiredService<IMergedServiceProviderFactory>();

            var scope = self.CreateScope();

            var builder = factory.CreateBuilder(services);
            builder.SourceProvider = scope.ServiceProvider;

            var newProvider = factory.CreateServiceProvider(builder);
            return new RelayServiceScope(newProvider, () =>
            {
                scope.Dispose();
                if (newProvider is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            });
        }
    }
}
