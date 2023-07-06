using System.Linq.Expressions;
using System.Reflection;
using Sarachan.UiHosting.Mvvm.Disposable;

namespace Sarachan.UiHosting.Mvvm.Events
{
    public interface IEventEntry
    {
        Type HandlerType { get; }

        IDisposable Subscribe(Delegate handler);
    }

    public interface IEventEntry<THandler> : IEventEntry
        where THandler : Delegate
    {
        IDisposable Subscribe(THandler handler);

        Type IEventEntry.HandlerType => typeof(THandler);

        IDisposable IEventEntry.Subscribe(Delegate handler)
        {
            return Subscribe(EventUtils.Cast<THandler>(handler));
        }
    }
}
 