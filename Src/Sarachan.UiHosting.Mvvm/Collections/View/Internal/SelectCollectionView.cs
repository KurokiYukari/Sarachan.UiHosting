using System.Collections.Specialized;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using Sarachan.UiHosting.Mvvm.Buffers;
using Sarachan.UiHosting.Mvvm.Events;

namespace Sarachan.UiHosting.Mvvm.Collections.View.Internal
{
    public sealed class SelectCollectionView<T, TView> : CollectionViewBase<T, TView>
    {
        public Func<T, TView> Selector { get; }

        public SelectCollectionView(IReadOnlyObservableCollection<T> collection,
            Func<INotifyCollectionChanged<T>, IEventEntry<NotifyCollectionChangedEventHandler<T>>>? entryCreator,
            Func<T, TView> selector)
            : base(collection, entryCreator)
        {
            Selector = selector;

            Refresh();
        }

        protected override void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs<T> e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        using var spanOwner = SpanOwner<TView>.Allocate(e.NewItems.Length);
                        for (int i = 0; i < e.NewItems.Length; i++)
                        {
                            spanOwner.Span[i] = Selector(e.NewItems[i]);
                        }
                        Storage.Insert(e.NewStartingIndex, spanOwner.Span);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Storage.RemoveAt(e.OldStartingIndex, e.OldItems.Length);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Storage[e.NewStartingIndex] = Selector(e.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Move:
                    Storage.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Refresh();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void Refresh()
        {
            using var spanOwner = SpanOwner.Allocate(Collection.Select(Selector));
            Storage.Reset(spanOwner.Span);
        }
    }
}
