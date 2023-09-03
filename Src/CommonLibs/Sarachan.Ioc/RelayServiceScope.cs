using Microsoft.Extensions.DependencyInjection;

namespace Sarachan.Ioc
{
    sealed class RelayServiceScope : IServiceScope
    {
        private readonly Action _disposeAction;
        
        public IServiceProvider ServiceProvider { get; }

        public RelayServiceScope(
            IServiceProvider provider,
            Action disposeAction)
        {
            ServiceProvider = provider;
            _disposeAction = disposeAction;
        }

        public void Dispose()
        {
            _disposeAction();
        }
    }
}
