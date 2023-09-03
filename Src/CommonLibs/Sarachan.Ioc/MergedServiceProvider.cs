using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Sarachan.Ioc
{
    sealed class MergedServiceProvider : 
        IServiceProvider, 
        IServiceProviderIsService,
        IServiceScopeFactory,
        IDisposable
    {
        private readonly IServiceProvider _sourceProvider;
        private readonly IServiceProviderIsService _sourceIsService;
        private readonly IServiceProvider _targetProvider;
        private readonly IServiceProviderIsService _targetIsService;

        private readonly ConcurrentDictionary<IServiceScope, byte> _scopes = new();

        public MergedServiceProvider(IServiceProvider sourceProvider, IServiceProvider targetProvider)
        {
            _sourceProvider = sourceProvider;
            _sourceIsService = _sourceProvider.GetRequiredService<IServiceProviderIsService>();
            _targetProvider = targetProvider;
            _targetIsService = _targetProvider.GetRequiredService<IServiceProviderIsService>();
        }

        public static MergedServiceProvider Create(IServiceProvider sourceProvider,
            IServiceProviderFactory<IServiceCollection> factory,
            IServiceCollection services)
        {
            return new MergedServiceProvider(sourceProvider, factory.CreateServiceProvider(factory.CreateBuilder(services)));
        }

        public bool IsService(Type serviceType)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            return _sourceIsService.IsService(serviceType) ||
                _targetIsService.IsService(serviceType);
        }

        public object? GetService(Type serviceType)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (serviceType == typeof(IServiceProvider) ||
                serviceType == typeof(IServiceProviderIsService) ||
                serviceType == typeof(IServiceScopeFactory))
            {
                return this;
            }

            object? result = _targetProvider.GetService(serviceType);
            result ??= _sourceProvider.GetService(serviceType);
            return result;
        }

        public IServiceScope CreateScope()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            var sourceScope = _sourceProvider.CreateScope();
            var targetScope = _targetProvider.CreateScope();

            var provider = new MergedServiceProvider(sourceScope.ServiceProvider, targetScope.ServiceProvider);

            IServiceScope scope = null!;
            scope = new RelayServiceScope(provider, () =>
            {
                sourceScope.Dispose();
                targetScope.Dispose();

                _scopes.TryRemove(scope, out _);
            });

            bool result = _scopes.TryAdd(scope, 0);
            Debug.Assert(result);
            return scope;
        }

        private bool _disposed;
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            foreach (var (scope, _) in _scopes)
            {
                scope.Dispose();
            }

            if (_targetProvider is IDisposable targetDisposable)
            {
                targetDisposable.Dispose();
            }
            if (_sourceProvider is IDisposable sourceDisposable)
            {
                sourceDisposable.Dispose();
            }
        }
    }
}
