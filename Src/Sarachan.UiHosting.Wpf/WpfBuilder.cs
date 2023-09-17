using Microsoft.Extensions.DependencyInjection;
using Sarachan.UiHosting.Extensions;

namespace Sarachan.UiHosting.Wpf.Extensions
{
    public interface IWpfBuilder
    {
    }

    internal sealed class WpfBuilder : IWpfBuilder
    {
        public IServiceCollection Services { get; }

        public WpfBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
