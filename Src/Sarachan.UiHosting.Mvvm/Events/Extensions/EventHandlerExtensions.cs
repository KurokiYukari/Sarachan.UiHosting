using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarachan.UiHosting.Mvvm.Events.Extensions
{
    public static class EventHandlerExtensions
    {
        public static IDisposable SubscribeWeak<TEventArgs>(this IEventEntry<EventHandler<TEventArgs>> self, object target, Func<object, EventHandler<TEventArgs>> handlerGetter)
        {
            return WeakEvent.SubscribeWeak<EventHandler<TEventArgs>, object, TEventArgs>(self, target, handlerGetter);
        }

        public static IDisposable SubscribeWeak<TEventArgs>(this IEventEntry<EventHandler<TEventArgs>> self, EventHandler<TEventArgs> handler)
        {
            return WeakEvent.SubscribeWeak<EventHandler<TEventArgs>, object, TEventArgs>(self, handler);
        }

        public static IEventEntry<EventHandler<TEventArgs>> ToWeak<TEventArgs>(this IEventEntry<EventHandler<TEventArgs>> self)
        {
            return WeakEvent.ToWeak<EventHandler<TEventArgs>, object, TEventArgs>(self);
        }
    }
}
