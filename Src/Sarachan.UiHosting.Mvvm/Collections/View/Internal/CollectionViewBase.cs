using System.Collections;
using Sarachan.UiHosting.Mvvm.Collections.Extensions;
using Sarachan.UiHosting.Mvvm.Events;

namespace Sarachan.UiHosting.Mvvm.Collections.View.Internal
{
    public abstract class CollectionViewBase<T, TView> : ICollectionView<T, TView>
    {
        public IReadOnlyObservableCollection<T> Collection { get; }

        protected ObservableList<TView> Storage { get; } = new();

        private IDisposable? _registration;

        public int Count => Storage.Count;

        public event NotifyCollectionChangedEventHandler<TView>? CollectionChanged;

        public CollectionViewBase(IReadOnlyObservableCollection<T> collection,
            Func<INotifyCollectionChanged<T>, IEventEntry<NotifyCollectionChangedEventHandler<T>>>? entryCreator)
        {
            Collection = collection;

            Storage.CollectionChanged += OnCollectionChanged;

            var entry = entryCreator?.Invoke(Collection) ?? Collection.CreateCollectionChangedEntry().ToWeak();
            _registration = entry.Subscribe(OnSourceCollectionChanged);
        }

        protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs<TView> e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        protected abstract void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs<T> e);

        public IEnumerator<TView> GetEnumerator()
        {
            return Storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _registration?.Dispose();
                    _registration = null;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
