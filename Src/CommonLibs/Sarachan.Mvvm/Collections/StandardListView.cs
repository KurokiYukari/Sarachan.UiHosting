using System.Collections;
using System.Collections.Specialized;
using Sarachan.Mvvm.Collections.Extensions;

namespace Sarachan.Mvvm.Collections
{

    public interface IStandardListView<T> : IReadOnlyList<T>, INotifyCollectionChanged, IDisposable
    {
        IReadOnlyObservableList<T> Collection { get; }
    }

    class StandardListView<T, TList> : IStandardListView<T> where TList : IReadOnlyObservableList<T>
    {
        private IDisposable? _registration;

        IReadOnlyObservableList<T> IStandardListView<T>.Collection => Collection;
        public TList Collection { get; }

        private readonly bool _supportRangeAction;

        public int Count => Collection.Count;

        public T this[int index] => Collection[index];

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public StandardListView(TList collection, bool supportRangeAction)
        {
            Collection = collection;
            _supportRangeAction = supportRangeAction;

            _registration = Collection.CreateCollectionChangedEntry()
                .ToWeak()
                .Subscribe(Collection_CollectionChanged);
        }

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs<T> e)
        {
            var standardArgs = e.ToStandardEventArgs(_supportRangeAction);
            CollectionChanged?.Invoke(this, standardArgs);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_registration != null)
            {
                if (disposing)
                {
                    _registration.Dispose();
                }

                _registration = null;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
