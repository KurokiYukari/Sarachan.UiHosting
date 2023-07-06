using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Sarachan.UiHosting.Mvvm.Collections
{
    public class ObservableDictionary<TKey, TValue> : ObservableCollectionBase<KeyValuePair<TKey, TValue>, OrderedDictionary<TKey, TValue>>,
        IDictionary<TKey, TValue>,
        IList<KeyValuePair<TKey, TValue>>,
        IReadOnlyList<KeyValuePair<TKey, TValue>>
        where TKey : notnull
    {
        public ObservableDictionary(IEqualityComparer<TKey> comparer) : base(new OrderedDictionary<TKey, TValue>(comparer))
        {
        }

        public ObservableDictionary() : this(EqualityComparer<TKey>.Default)
        {

        }

        public TValue this[TKey key]
        {
            get => Storage[key];
            set
            {
                var index = IndexOf(key);
                if (index >= 0)
                {
                    this[index] = KeyValuePair.Create(key, value);
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        public ICollection<TKey> Keys => Storage.Keys;

        public ICollection<TValue> Values => Storage.Values;

        public KeyValuePair<TKey, TValue> this[int index]
        {
            get => Storage[index];
            set
            {
                var oldPair = Storage[index];
                if (!EqualityComparer<KeyValuePair<TKey, TValue>>.Default.Equals(oldPair, value))
                {
                    Storage[index] = value;
                    OnCollectionChanged(NotifyCollectionChangedEventArgs<KeyValuePair<TKey, TValue>>.Replace(value, oldPair, index));
                }
            }
        }

        public virtual void Add(TKey key, TValue value)
        {
            var index = Count;
            Storage.Add(key, value);
            OnCollectionChanged(NotifyCollectionChangedEventArgs<KeyValuePair<TKey, TValue>>.Add(KeyValuePair.Create(key, value), index));
        }

        public sealed override void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Insert(int index, KeyValuePair<TKey, TValue> item)
        {
            Storage.Insert(index, item);
            OnCollectionChanged(NotifyCollectionChangedEventArgs<KeyValuePair<TKey, TValue>>.Add(item, index));
        }

        public virtual bool Remove(TKey key)
        {
            var index = IndexOf(key);
            if (index < 0)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        public sealed override bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public void RemoveAt(int index)
        {
            var oldValue = Storage[index];
            Storage.RemoveAt(index);
            OnCollectionChanged(NotifyCollectionChangedEventArgs<KeyValuePair<TKey, TValue>>.Remove(oldValue, index));
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            return Storage.TryGetValue(key, out value);
        }

        public bool ContainsKey(TKey key)
        {
            return Storage.ContainsKey(key);
        }

        public int IndexOf(TKey key)
        {
            return Storage.IndexOf(key);
        }

        public int IndexOf(KeyValuePair<TKey, TValue> item)
        {
            return Storage.IndexOf(item);
        }
    }
}
