using System.Collections.Specialized;
using Sarachan.UiHosting.Mvvm.Events;

namespace Sarachan.UiHosting.Mvvm.Collections.View.Internal
{
    public sealed class WhereCollectionView<T> : CollectionViewBase<T, T>
    {
        public Predicate<T> Filter { get; }

        private readonly List<int> _indexMap = new();

        public WhereCollectionView(IReadOnlyObservableCollection<T> collection,
            Func<INotifyCollectionChanged<T>, IEventEntry<NotifyCollectionChangedEventHandler<T>>>? entryCreator,
            Predicate<T> filter) : base(collection, entryCreator)
        {
            Filter = filter;

            Refresh();
        }

        protected override void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs<T> e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var newIndex = e.NewStartingIndex;
                        foreach (var item in e.NewItems)
                        {
                            if (Filter?.Invoke(item) ?? true)
                            {
                                GetViewInsertIndex(newIndex, out var viewIndex);
                                InsertViewItem(newIndex, viewIndex, item);
                            }
                            else
                            {
                                _indexMap.Insert(newIndex, -1);
                            }

                            newIndex++;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        for (int i = 0; i < e.OldItems.Length; i++)
                        {
                            var viewIndex = _indexMap[i + e.OldStartingIndex];
                            if (viewIndex >= 0)
                            {
                                RemoveViewItem(viewIndex);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        var replaceNewItem = e.NewItems[0];
                        bool oldExisted = GetViewInsertIndex(e.NewStartingIndex, out var replaceViewIndex);
                        bool needAddNew = Filter?.Invoke(replaceNewItem) ?? true;
                        if (oldExisted && needAddNew)
                        {
                            Storage[replaceViewIndex] = replaceNewItem;
                        }
                        else if (oldExisted)
                        {
                            RemoveViewItem(replaceViewIndex);
                        }
                        else if (needAddNew)
                        {
                            InsertViewItem(e.NewStartingIndex, replaceViewIndex, replaceNewItem);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    {
                        var fromViewIndex = _indexMap[e.OldStartingIndex];
                        if (fromViewIndex >= 0)
                        {
                            bool toViewItemExisted = GetViewInsertIndex(e.NewStartingIndex, out var toViewIndex);
                            if (fromViewIndex < toViewIndex)
                            {
                                if (!toViewItemExisted)
                                {
                                    toViewIndex--;
                                }
                                for (int i = 0; i < _indexMap.Count; i++)
                                {
                                    var mappedIndex = _indexMap[i];
                                    if (mappedIndex >= fromViewIndex && mappedIndex <= toViewIndex)
                                    {
                                        _indexMap[i] = mappedIndex - 1;
                                    }
                                }
                            }
                            else if (fromViewIndex > toViewIndex)
                            {
                                for (int i = 0; i < _indexMap.Count; i++)
                                {
                                    var mappedIndex = _indexMap[i];
                                    if (mappedIndex >= toViewIndex && mappedIndex <= fromViewIndex)
                                    {
                                        _indexMap[i] = mappedIndex + 1;
                                    }
                                }
                            }

                            Storage.Move(fromViewIndex, toViewIndex);
                            _indexMap[e.OldStartingIndex] = toViewIndex;
                        }
                        CollectionUtils.Move(_indexMap, e.OldStartingIndex, e.NewStartingIndex);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Refresh();
                    break;
                default:
                    break;
            }
        }

        private void InsertViewItem(int newIndex, int viewIndex, T item)
        {
            for (int j = 0; j < _indexMap.Count; j++)
            {
                var mappedIndex = _indexMap[j];
                if (mappedIndex >= viewIndex)
                {
                    _indexMap[j] = mappedIndex + 1;
                }
            }
            _indexMap.Insert(newIndex, viewIndex);
            Storage.Insert(viewIndex, item);
        }

        private void RemoveViewItem(int viewIndex)
        {
            Storage.RemoveAt(viewIndex);
            for (int j = 0; j < _indexMap.Count; j++)
            {
                var mappedIndex = _indexMap[j];
                if (mappedIndex > viewIndex)
                {
                    _indexMap[j] = mappedIndex - 1;
                }
            }
        }

        private bool GetViewInsertIndex(int originIndex, out int viewIndex)
        {
            if (originIndex >= _indexMap.Count)
            {
                viewIndex = Storage.Count;
                return false;
            }

            viewIndex = _indexMap[originIndex];
            if (viewIndex >= 0)
            {
                return true;
            }

            for (int i = originIndex - 1; i >= 0; i--)
            {
                viewIndex = _indexMap[i];
                if (viewIndex >= 0)
                {
                    viewIndex++;
                    return false;
                }
            }

            for (int i = originIndex + 1; i < _indexMap.Count; i++)
            {
                viewIndex = _indexMap[i];
                if (viewIndex >= 0)
                {
                    return false;
                }
            }

            viewIndex = 0;
            return false;
        }

        public void Refresh()
        {
            Storage.Clear();
            _indexMap.Clear();

            foreach (var item in Collection)
            {
                if (Filter?.Invoke(item) ?? true)
                {
                    _indexMap.Add(Storage.Count);
                    Storage.Add(item);
                }
                else
                {
                    _indexMap.Add(-1);
                }
            }
        }
    }
}
