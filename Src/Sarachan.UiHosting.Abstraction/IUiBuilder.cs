using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sarachan.UiHosting.Extensions
{
    public interface IUiBuilder
    {
        IServiceCollection Services { get; }
    }
}