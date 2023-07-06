namespace Sarachan.UiHosting.Mvvm.Events
{
    public static class EventUtils
    {
        public static THandler Cast<THandler>(Delegate @delegate) where THandler : Delegate
        {
            return (THandler)Cast(@delegate, typeof(THandler));
        }

        public static Delegate Cast(Delegate @delegate, Type targetType)
        {
            if (@delegate.GetType() == targetType)
            {
                return @delegate;
            }
            return Delegate.CreateDelegate(targetType, @delegate.Target, @delegate.Method);
        }

        public static IEventEntry<THandler> Create<THandler>(Action<THandler> addHandlerAction, Action<THandler> removeHandlerAction) where THandler : Delegate
        {
            return new GenericEventEntry<THandler>(addHandlerAction, removeHandlerAction);
        }

        sealed class GenericEventEntry<THandler> : IEventEntry<THandler> where THandler : Delegate
        {
            private readonly Action<THandler> _addHandlerAction;
            private readonly Action<THandler> _removeHandlerAction;

            public GenericEventEntry(Action<THandler> addHandlerAction, Action<THandler> removeHandlerAction)
            {
                _addHandlerAction = addHandlerAction;
                _removeHandlerAction = removeHandlerAction;
            }

            public IDisposable Subscribe(THandler handler)
            {
                _addHandlerAction(handler);
                return new UnsubscribeHandlerDisposable(this, handler);
            }

            sealed class UnsubscribeHandlerDisposable : IDisposable
            {
                private readonly GenericEventEntry<THandler> _entry;
                private readonly WeakReference<THandler> _weakHandler;

                public UnsubscribeHandlerDisposable(GenericEventEntry<THandler> entry, THandler handler)
                {
                    _entry = entry;
                    _weakHandler = new(handler);
                }

                private bool _disposed;
                public void Dispose()
                {
                    if (_disposed)
                    {
                        return;
                    }

                    if (_weakHandler.TryGetTarget(out var handler))
                    {
                        _entry._removeHandlerAction(handler);
                    }

                    _disposed = true;
                }
            }
        }

        public static IEventEntry<THandler> Create<THandler>(Func<THandler, IDisposable> subscribeAction) where THandler : Delegate
        {
            return new RelayEventEntry<THandler>(subscribeAction);
        }

        sealed class RelayEventEntry<THandler> : IEventEntry<THandler> where THandler : Delegate
        {
            private readonly Func<THandler, IDisposable> _subscribeAction;

            public RelayEventEntry(Func<THandler, IDisposable> subscribeAction)
            {
                _subscribeAction = subscribeAction;
            }

            public IDisposable Subscribe(THandler handler)
            {
                return _subscribeAction(handler);
            }
        }

        public static IEventEntry<THandler> Cast<THandler>(this IEventEntry self)
            where THandler : Delegate
        {
            return new CastEventEntry<THandler>(self);
        }

        sealed class CastEventEntry<THandler> : IEventEntry<THandler>
            where THandler : Delegate
        {
            private readonly IEventEntry _entry;

            public CastEventEntry(IEventEntry entry)
            {
                _entry = entry;
            }

            public IDisposable Subscribe(THandler handler)
            {
                var originHandler = Cast(handler, _entry.HandlerType);
                return _entry.Subscribe(originHandler);
            }
        }
    }
}
 