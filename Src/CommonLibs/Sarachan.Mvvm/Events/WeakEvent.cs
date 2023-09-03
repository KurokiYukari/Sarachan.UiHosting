using System;
using System.Reflection;

namespace Sarachan.Mvvm.Events
{
    public static class WeakEvent
    {
        sealed class EventUnit<TSender, TEventArgs>
        {
            private readonly WeakReference<object> _weakTarget;
            private readonly Action<object, TSender, TEventArgs> _handler;

            public event Action<EventUnit<TSender, TEventArgs>>? Detached;

            public EventUnit(object target, Action<object, TSender, TEventArgs> handler)
            {
                _weakTarget = new WeakReference<object>(target);
                _handler = handler;
            }

            public void OnEvent(TSender sender, TEventArgs e)
            {
                if (_weakTarget.TryGetTarget(out var target))
                {
                    _handler(target, sender, e);
                }
                else
                {
                    Detached?.Invoke(this);
                }
            }
        }

        public static IDisposable SubscribeWeak<THandler, TSender, TEventArgs>(IEventEntry<THandler> self,
            object target,
            Func<object, THandler> handlerGetter) 
            where THandler : Delegate
        {
            var unit = new EventUnit<TSender, TEventArgs>(target, (target, sender, e) =>
            {
                var handler = handlerGetter(target);
                handler.DynamicInvoke(sender, e);
            });

            var handler = EventUtils.Cast<THandler>(unit.OnEvent);
            var registration = self.Subscribe(handler);
            unit.Detached += _ => registration.Dispose();
            return registration;
        }

        public static IDisposable SubscribeWeak<THandler, TSender, TEventArgs>(IEventEntry<THandler> self,
            THandler handler)
            where THandler : Delegate
        {
            var target = handler.Target;
            var method = handler.Method;
            if (target == null)
            {
                return self.Subscribe(handler);
            }

            return SubscribeWeak<THandler, TSender, TEventArgs>(self, target, target =>
            {
                return (THandler)Delegate.CreateDelegate(typeof(THandler), target, method);
            });
        }

        public static IEventEntry<THandler> ToWeak<THandler, TSender, TEventArgs>(IEventEntry<THandler> self) where THandler : Delegate
        {
            return EventUtils.Create<THandler>(h => SubscribeWeak<THandler, TSender, TEventArgs>(self, h));
        }
    }
}
 