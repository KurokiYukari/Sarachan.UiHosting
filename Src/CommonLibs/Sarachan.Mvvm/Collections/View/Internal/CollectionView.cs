using System.Collections;
using System.Collections.Specialized;
using Sarachan.Mvvm.Buffers;
using Sarachan.Mvvm.Collections.Extensions;
using Sarachan.Mvvm.Events;

namespace Sarachan.Mvvm.Collections.View.Internal
{
    sealed class CollectionView<T, TView> : ICollectionView<T, TView>
    {
        public IReadOnlyObservableCollection<T> Collection { get; }
        private readonly CollectionView.IEventEmitter<T, TView> _emitter;

        private readonly ObservableList<TView> _storage = new();

        private IDisposable? _registration;

        public int Count => _storage.Count;

        public TView this[int index] => _storage[index];

        public event NotifyCollectionChangedEventHandler<TView>? CollectionChanged;

        public CollectionView(IReadOnlyObservableCollection<T> collection,
            Func<INotifyCollectionChanged<T>, IEventEntry<NotifyCollectionChangedEventHandler<T>>>? entryCreator,
            CollectionView.IEventEmitter<T, TView> emitter)
        {
            Collection = collection;
            _emitter = emitter;

            _storage.CollectionChanged += OnCollectionChanged;

            var entry = entryCreator?.Invoke(Collection) ?? Collection.CreateCollectionChangedEntry().ToWeak();
            _registration = entry.Subscribe(OnSourceCollectionChanged);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs<TView> e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs<T> e)
        {
            _emitter.Emit(e, OnCollectionChangedEmitted);
        }

        public void Refresh()
        {
            using var spanOwner = SpanOwner.Allocate(Collection);
            var e = NotifyCollectionChangedEventArgs<T>.Reset(spanOwner.Span, default);
            _emitter.Emit(e, OnCollectionChangedEmitted);
        }

        private void OnCollectionChangedEmitted(object sender, NotifyCollectionChangedEventArgs<TView> e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _storage.Insert(e.NewStartingIndex, e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _storage.RemoveAt(e.OldStartingIndex, e.OldItems.Length);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    _storage[e.NewStartingIndex] = e.NewItems[0];
                    break;
                case NotifyCollectionChangedAction.Move:
                    _storage.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _storage.Reset(e.NewItems);
                    break;
                default:
                    break;
            }
        }

        public IEnumerator<TView> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool _disposed;
        private void Dispose(bool disposing)
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
