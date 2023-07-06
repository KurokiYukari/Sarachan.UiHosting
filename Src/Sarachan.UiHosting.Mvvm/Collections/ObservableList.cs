namespace Sarachan.UiHosting.Mvvm.Collections
{
    public interface IReadOnlyObservableList<T> : IReadOnlyObservableCollection<T>, IReadOnlyList<T>
    {
    }

    public interface IObservableList<T> : IObservableCollection<T>, IReadOnlyObservableList<T>, IList<T>
    {
    }

    public class ObservableList<T> : ObservableCollectionBase<T, IList<T>>, IObservableList<T>
    {
        public T this[int index]
        {
            get => Storage[index];
            set
            {
                var oldValue = Storage[index];
                Storage[index] = value;
                OnCollectionChanged(NotifyCollectionChangedEventArgs<T>.Replace(value, oldValue, index));
            }
        }

        public ObservableList() : base(new List<T>())
        {
        }

        public int IndexOf(T item)
        {
            return Storage.IndexOf(item);
        }

        public virtual void Insert(int index, T item)
        {
            Storage.Insert(index, item);
            OnCollectionChanged(NotifyCollectionChangedEventArgs<T>.Add(item, index));
        }

        public virtual void RemoveAt(int index)
        {
            var oldItem = Storage[index];
            Storage.RemoveAt(index);
            OnCollectionChanged(NotifyCollectionChangedEventArgs<T>.Remove(oldItem, index));
        }

        public sealed override void Add(T item)
        {
            Insert(Count, item);
        }

        public sealed override bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index < 0)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }
    }
}
