using System.Collections.Specialized;
using System.Reflection;
using Sarachan.UiHosting.Mvvm.Disposable;
using Sarachan.UiHosting.Mvvm.Events;

namespace Sarachan.UiHosting.Mvvm.Collections.Extensions
{
    public static class NotifyCollectionExtensions
    {
        // because NotifyCollectionChangedEventArgs<T> is a ref struct type, we cannot use it as generic parameter

        public static IEventEntry<NotifyCollectionChangedEventHandler<T>> CreateCollectionChangedEntry<T>(this INotifyCollectionChanged<T> self)
        {
            return EventUtils.Create<NotifyCollectionChangedEventHandler<T>>(
                h => self.CollectionChanged += h,
                h => self.CollectionChanged -= h);
        }

        #region Weak Event
        public delegate void StaticHandler<T>(object target, object sender, NotifyCollectionChangedEventArgs<T> e);

        sealed class WeakEventUnit<T>
        {
            private readonly WeakReference<object> _weakTarget;
            private readonly StaticHandler<T> _handler;

            public event Action<WeakEventUnit<T>>? Detached;

            public WeakEventUnit(object target, StaticHandler<T> handler)
            {
                _weakTarget = new WeakReference<object>(target);
                _handler = handler;
            }

            public void OnEvent(object sender, NotifyCollectionChangedEventArgs<T> e)
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

        public static IDisposable SubscribeWeak<T>(this IEventEntry<NotifyCollectionChangedEventHandler<T>> self,
            object target,
            StaticHandler<T> handler)
        {
            var unit = new WeakEventUnit<T>(target, handler);
            var registration = self.Subscribe(unit.OnEvent);
            unit.Detached += _ => registration.Dispose();
            return registration;
        }

        public static IDisposable SubscribeWeak<T>(this IEventEntry<NotifyCollectionChangedEventHandler<T>> self,
            NotifyCollectionChangedEventHandler<T> handler)
        {
            var target = handler.Target;
            var method = handler.Method;

            if (target == null)
            {
                return self.Subscribe(handler);
            }

            return self.SubscribeWeak(target, (target, sender, e) =>
            {
                var handler = (NotifyCollectionChangedEventHandler<T>)Delegate.CreateDelegate(typeof(NotifyCollectionChangedEventHandler<T>), target, method);
                handler(sender, e);
            });
        }

        public static IEventEntry<NotifyCollectionChangedEventHandler<T>> ToWeak<T>(this IEventEntry<NotifyCollectionChangedEventHandler<T>> self)
        {
            return EventUtils.Create<NotifyCollectionChangedEventHandler<T>>(h => self.SubscribeWeak(h));
        }
        #endregion

        public static IEventEntry<NotifyCollectionChangedEventHandler<T>> EachAdded<T>(this IEventEntry<NotifyCollectionChangedEventHandler<T>> self, Action<T> action)
        {
            self.Subscribe((sender, e) =>
            {
                foreach (var item in e.NewItems)
                {
                    action(item);
                }
            });
            return self;
        }

        public static IEventEntry<NotifyCollectionChangedEventHandler<T>> EachRemoved<T>(this IEventEntry<NotifyCollectionChangedEventHandler<T>> self, Action<T> action)
        {
            self.Subscribe((sender, e) =>
            {
                foreach (var item in e.OldItems)
                {
                    action(item);
                }
            });
            return self;
        }

        //public static 
    }
}