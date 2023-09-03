using System.Collections.Specialized;

namespace Sarachan.Mvvm.Events.Extensions
{
    public static class CollectionChangedEventExtensions
    {
        public static IEventEntry<EventHandler<NotifyCollectionChangedEventArgs>> CreateCollectionChangedEntry(this INotifyCollectionChanged self)
        {
            return EventUtils.Create<NotifyCollectionChangedEventHandler>(
                h => self.CollectionChanged += h,
                h => self.CollectionChanged -= h)
                .Cast<EventHandler<NotifyCollectionChangedEventArgs>>();
        }
    }
}
