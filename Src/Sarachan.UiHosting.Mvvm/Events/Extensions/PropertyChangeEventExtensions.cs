using System.ComponentModel;

namespace Sarachan.UiHosting.Mvvm.Events.Extensions
{
    public static class PropertyChangeEventExtensions
    {
        public static IEventEntry<EventHandler<PropertyChangedEventArgs>> CreatePropertyChangedEntry(this INotifyPropertyChanged self)
        {
            return EventUtils.Create<PropertyChangedEventHandler>(
                h => self.PropertyChanged += h,
                h => self.PropertyChanged -= h)
                .Cast<EventHandler<PropertyChangedEventArgs>>();
        }

        public static IEventEntry<EventHandler<PropertyChangingEventArgs>> CreatePropertyChangingEntry(this INotifyPropertyChanging self)
        {
            return EventUtils.Create<PropertyChangingEventHandler>(
                h => self.PropertyChanging += h,
                h => self.PropertyChanging -= h)
                .Cast<EventHandler<PropertyChangingEventArgs>>();
        }
    }
}
