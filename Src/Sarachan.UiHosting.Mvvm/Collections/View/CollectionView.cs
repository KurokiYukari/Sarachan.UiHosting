using System;
using System.Collections;
using System.Collections.Specialized;
using Sarachan.UiHosting.Mvvm.Collections.View.Internal;
using Sarachan.UiHosting.Mvvm.Events;

namespace Sarachan.UiHosting.Mvvm.Collections.View
{
    public interface ICollectionView<T, TView> : IReadOnlyObservableCollection<TView>, IDisposable
    {
        IReadOnlyObservableCollection<T> Collection { get; }
    }

    public static class CollectionView
    {
        public static ICollectionView<T, TView> CreateSelectView<T, TView>(
            this IReadOnlyObservableCollection<T> self,
            Func<T, TView> selector,
            Func<INotifyCollectionChanged<T>, IEventEntry<NotifyCollectionChangedEventHandler<T>>>? entryCreator = null)
        {
            return new SelectCollectionView<T, TView>(self, entryCreator, selector);
        }

        public static ICollectionView<T, T> CreateWhereView<T>(
            this IReadOnlyObservableCollection<T> self,
            Predicate<T> predicate,
            Func<INotifyCollectionChanged<T>, IEventEntry<NotifyCollectionChangedEventHandler<T>>>? entryCreator = null)
        {
            return new WhereCollectionView<T>(self, entryCreator, predicate);
        }
    }

    //public class CollectionView<T, TView> : ICollectionView<T, TView>
    //{
    //    public IReadOnlyObservableCollection<T> Collection { get; }

    //    private readonly ObservableList<TView> _views = new();
    //    private readonly Func<T, TView> _transformer;

    //    public Predicate<T>? Filter { get; set; }

    //    private readonly List<int> _indexMap = new();

    //    private IDisposable? _registration;

    //    public int Count => _views.Count;

    //    public event NotifyCollectionChangedEventHandler<TView>? CollectionChanged;

    //    public CollectionView(IReadOnlyObservableCollection<T> collection,
    //        Func<T, TView> transformer,
    //        Func<INotifyCollectionChanged<T>, IEventEntry<NotifyCollectionChangedEventHandler<T>>> entryCreator)
    //    {
    //        Collection = collection;
    //        _transformer = transformer;

    //        _registration = entryCreator(Collection)
    //            .Subscribe(OnSourceCollectionChanged);

    //        Refresh();
    //    }

    //    private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs<T> e)
    //    {
    //        switch (e.Action)
    //        {
    //            case NotifyCollectionChangedAction.Add:
    //                {
    //                    var newIndex = e.NewStartingIndex;
    //                    foreach (var item in e.NewItems)
    //                    {
    //                        if (Filter?.Invoke(item) ?? true)
    //                        {
    //                            GetViewInsertIndex(newIndex, out var viewIndex);
    //                            InsertViewItem(newIndex, viewIndex, _transformer(item));
    //                        }
    //                        else
    //                        {
    //                            _indexMap.Insert(newIndex, -1);
    //                        }

    //                        newIndex++;
    //                    }
    //                }
    //                break;
    //            case NotifyCollectionChangedAction.Remove:
    //                {
    //                    for (int i = 0; i < e.OldItems.Length; i++)
    //                    {
    //                        var viewIndex = _indexMap[i + e.OldStartingIndex];
    //                        if (viewIndex >= 0)
    //                        {
    //                            RemoveViewItem(viewIndex);
    //                        }
    //                    }
    //                }
    //                break;
    //            case NotifyCollectionChangedAction.Replace:
    //                {
    //                    var replaceNewItem = e.NewItems[0];
    //                    bool oldExisted = GetViewInsertIndex(e.NewStartingIndex, out var replaceViewIndex);
    //                    bool needAddNew = Filter?.Invoke(replaceNewItem) ?? true;
    //                    if (oldExisted && needAddNew)
    //                    {
    //                        _views[replaceViewIndex] = _transformer(replaceNewItem);
    //                    }
    //                    else if (oldExisted)
    //                    {
    //                        RemoveViewItem(replaceViewIndex);
    //                    }
    //                    else if (needAddNew)
    //                    {
    //                        InsertViewItem(e.NewStartingIndex, replaceViewIndex, _transformer(replaceNewItem));
    //                    }
    //                }
    //                break;
    //            case NotifyCollectionChangedAction.Move:
    //                {
    //                    var fromViewIndex = _indexMap[e.OldStartingIndex];
    //                    if (fromViewIndex >= 0)
    //                    {
    //                        bool toViewItemExisted = GetViewInsertIndex(e.NewStartingIndex, out var toViewIndex);
    //                        if (fromViewIndex < toViewIndex)
    //                        {
    //                            if (!toViewItemExisted)
    //                            {
    //                                toViewIndex--;
    //                            }
    //                            for (int i = 0; i < _indexMap.Count; i++)
    //                            {
    //                                var mappedIndex = _indexMap[i];
    //                                if (mappedIndex >= fromViewIndex && mappedIndex <= toViewIndex)
    //                                {
    //                                    _indexMap[i] = mappedIndex - 1;
    //                                }
    //                            }
    //                        }
    //                        else if (fromViewIndex > toViewIndex)
    //                        {
    //                            for (int i = 0; i < _indexMap.Count; i++)
    //                            {
    //                                var mappedIndex = _indexMap[i];
    //                                if (mappedIndex >= toViewIndex && mappedIndex <= fromViewIndex)
    //                                {
    //                                    _indexMap[i] = mappedIndex + 1;
    //                                }
    //                            }
    //                        }

    //                        _views.Move(fromViewIndex, toViewIndex);
    //                        _indexMap[e.OldStartingIndex] = toViewIndex;
    //                    }
    //                    CollectionUtils.Move(_indexMap, e.OldStartingIndex, e.NewStartingIndex);
    //                }
    //                break;
    //            case NotifyCollectionChangedAction.Reset:
    //                Refresh();
    //                break;
    //            default:
    //                break;
    //        }
    //    }

    //    private void InsertViewItem(int newIndex, int viewIndex, TView item)
    //    {
    //        for (int j = 0; j < _indexMap.Count; j++)
    //        {
    //            var mappedIndex = _indexMap[j];
    //            if (mappedIndex >= viewIndex)
    //            {
    //                _indexMap[j] = mappedIndex + 1;
    //            }
    //        }
    //        _indexMap.Insert(newIndex, viewIndex);
    //        _views.Insert(viewIndex, item);
    //    }

    //    private void RemoveViewItem(int viewIndex)
    //    {
    //        _views.RemoveAt(viewIndex);
    //        for (int j = 0; j < _indexMap.Count; j++)
    //        {
    //            var mappedIndex = _indexMap[j];
    //            if (mappedIndex > viewIndex)
    //            {
    //                _indexMap[j] = mappedIndex - 1;
    //            }
    //        }
    //    }

    //    private bool GetViewInsertIndex(int originIndex, out int viewIndex)
    //    {
    //        if (originIndex >= _indexMap.Count)
    //        {
    //            viewIndex = _views.Count;
    //            return false;
    //        }

    //        viewIndex = _indexMap[originIndex];
    //        if (viewIndex >= 0)
    //        {
    //            return true;
    //        }

    //        for (int i = originIndex - 1; i >= 0; i--)
    //        {
    //            viewIndex = _indexMap[i];
    //            if (viewIndex >= 0)
    //            {
    //                viewIndex++;
    //                return false;
    //            }
    //        }

    //        for (int i = originIndex + 1; i < _indexMap.Count; i++)
    //        {
    //            viewIndex = _indexMap[i];
    //            if (viewIndex >= 0)
    //            {
    //                return false;
    //            }
    //        }

    //        viewIndex = 0;
    //        return false;
    //    }

    //    public void Refresh()
    //    {
    //        _views.Clear();
    //        _indexMap.Clear();

    //        foreach (var item in Collection)
    //        {
    //            if (Filter?.Invoke(item) ?? true)
    //            {
    //                _indexMap.Add(_views.Count);
    //                var viewItem = _transformer(item);
    //                _views.Add(viewItem);
    //            }
    //            else
    //            {
    //                _indexMap.Add(-1);
    //            }
    //        }
    //    }

    //    public IEnumerator<TView> GetEnumerator()
    //    {
    //        return _views.GetEnumerator();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }

    //    public void Dispose()
    //    {
    //        _registration?.Dispose();
    //        _registration = null;
    //    }
    //}
}
