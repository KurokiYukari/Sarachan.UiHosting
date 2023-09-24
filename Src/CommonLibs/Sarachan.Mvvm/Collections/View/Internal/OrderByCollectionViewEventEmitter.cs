using System.Collections.Specialized;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance.Buffers;

namespace Sarachan.Mvvm.Collections.View.Internal
{
    class OrderByCollectionViewEventEmitter<T> : CollectionView.IEventEmitter<T, T>
    {
        private readonly ObservableList<T> _items = new();
        private readonly List<int> _originalToNewIndexMap = new();

        public Comparison<T> Comparison { get; }

        public OrderByCollectionViewEventEmitter(Comparison<T> comparison)
        {
            Comparison = comparison;
        }

        public void Emit(NotifyCollectionChangedEventArgs<T> e, NotifyCollectionChangedEventHandler<T> handler)
        {
            _items.CollectionChanged += handler;
            try
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            int index = e.NewStartingIndex;
                            for (int i = 0; i < e.NewItems.Length; i++)
                            {
                                InsertItem(index + i, e.NewItems[i]);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        {
                            int index = e.OldStartingIndex;
                            for (int i = e.OldItems.Length - 1; i >= 0 ; i--)
                            {
                                RemoveItem(index + i);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        RemoveItem(e.NewStartingIndex);
                        InsertItem(e.NewStartingIndex, e.NewItems[0]);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        Refresh(e.NewItems);
                        break;
                    case NotifyCollectionChangedAction.Move:
                    default:
                        break;
                }
            }
            finally
            {
                _items.CollectionChanged -= handler;
            }
        }

        private void Refresh(ReadOnlySpan<T> newItems)
        {
            _originalToNewIndexMap.Clear();

            Span<int> indexes = stackalloc int[newItems.Length];
            for (int i = 0; i < indexes.Length; i++)
            {
                indexes[i] = i;
            }

            using var itemsSpanOwner = SpanOwner<T>.Allocate(newItems.Length);
            var itemsSpan = itemsSpanOwner.Span;
            for (int i = 0; i < newItems.Length; i++)
            {
                itemsSpan[i] = newItems[i];
            }

            itemsSpan.Sort(indexes, Comparison);

            _items.Reset(itemsSpan);
            CollectionUtils.Resize(_originalToNewIndexMap, _items.Count);
            for (int i = 0; i < indexes.Length; i++)
            {
                _originalToNewIndexMap[indexes[i]] = i;
            }
        }

        private void RemoveItem(int originalIndex)
        {
            var newIndex = _originalToNewIndexMap[originalIndex];
            _items.RemoveAt(newIndex);
            _originalToNewIndexMap.RemoveAt(originalIndex);
            for (int i = 0; i < _originalToNewIndexMap.Count; i++)
            {
                var index = _originalToNewIndexMap[i];
                if (index > newIndex)
                {
                    _originalToNewIndexMap[i] = index - 1;
                }
            }
        }

        private void InsertItem(int originalIndex, T item)
        {
            var insertIndex = FindInsertIndex(item);

            _items.Insert(insertIndex, item);
            if (insertIndex < _items.Count)
            {
                for (int i = 0; i < _originalToNewIndexMap.Count; i++)
                {
                    var index = _originalToNewIndexMap[i];
                    if (index >= insertIndex)
                    {
                        _originalToNewIndexMap[i] = index + 1;
                    }
                }
            }
            _originalToNewIndexMap.Insert(originalIndex, insertIndex);
        }

        private int FindInsertIndex(T item)
        {
            var comparison = Comparison;

            int start = 0;
            int end = _items.Count - 1;

            while (start <= end)
            {
                int center = (start + end) / 2;
                var centerItem = _items[center];

                var comparisonResult = comparison(item, centerItem);
                if (comparisonResult > 0)
                {
                    start = center + 1;
                }
                else if (comparisonResult < 0)
                {
                    end = center - 1;
                }
                else
                {
                    return center;
                }
            }

            return start;
        }
    }
}
