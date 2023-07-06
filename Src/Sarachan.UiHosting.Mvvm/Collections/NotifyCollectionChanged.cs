using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace Sarachan.UiHosting.Mvvm.Collections
{
    public interface INotifyCollectionChanged<T>
    {
        event NotifyCollectionChangedEventHandler<T> CollectionChanged;
    }

    public delegate void NotifyCollectionChangedEventHandler<T>(object sender, NotifyCollectionChangedEventArgs<T> e);

    [StructLayout(LayoutKind.Auto)]
    public readonly ref struct NotifyCollectionChangedEventArgs<T>
    {
        public NotifyCollectionChangedAction Action { get; }
        public ReadOnlySpan<T> NewItems { get; }
        public ReadOnlySpan<T> OldItems { get; }
        public int NewStartingIndex { get; }
        public int OldStartingIndex { get; }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, ReadOnlySpan<T> newItems = default, ReadOnlySpan<T> oldItems = default, int newStartingIndex = -1, int oldStartingIndex = -1)
        {
            Action = action;
            NewItems = newItems;
            OldItems = oldItems;
            NewStartingIndex = newStartingIndex;
            OldStartingIndex = oldStartingIndex;
        }

        public NotifyCollectionChangedEventArgs ToStandardEventArgs(bool supportRangeAction)
        {
            if (!supportRangeAction)
            {
                if (NewItems.Length > 1 || OldItems.Length > 1)
                {
                    return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                }
            }

            return Action switch
            {
                NotifyCollectionChangedAction.Add => new NotifyCollectionChangedEventArgs(Action, NewItems.ToArray(), NewStartingIndex),
                NotifyCollectionChangedAction.Remove => new NotifyCollectionChangedEventArgs(Action, OldItems.ToArray(), OldStartingIndex),
                NotifyCollectionChangedAction.Replace => new NotifyCollectionChangedEventArgs(Action, NewItems.ToArray(), OldItems.ToArray(), NewStartingIndex),
                NotifyCollectionChangedAction.Move => new NotifyCollectionChangedEventArgs(Action, OldItems[0], NewStartingIndex, OldStartingIndex),
                NotifyCollectionChangedAction.Reset => new NotifyCollectionChangedEventArgs(Action),
                _ => throw new NotSupportedException(),
            };
        }

        public static NotifyCollectionChangedEventArgs<T> Add(in T newItem, int newStartingIndex)
        {
            return new NotifyCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Add, newItems: new(newItem), newStartingIndex: newStartingIndex);
        }

        public static NotifyCollectionChangedEventArgs<T> Add(ReadOnlySpan<T> newItems, int newStartingIndex)
        {
            return new NotifyCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Add, newItems: newItems, newStartingIndex: newStartingIndex);
        }

        public static NotifyCollectionChangedEventArgs<T> Remove(in T oldItem, int oldStartingIndex)
        {
            return new NotifyCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Remove, oldItems: new(oldItem), oldStartingIndex: oldStartingIndex);
        }

        public static NotifyCollectionChangedEventArgs<T> Remove(ReadOnlySpan<T> oldItems, int oldStartingIndex)
        {
            return new NotifyCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Remove, oldItems: oldItems, oldStartingIndex: oldStartingIndex);
        }

        public static NotifyCollectionChangedEventArgs<T> Replace(in T newItem, in T oldItem, int startingIndex)
        {
            return new NotifyCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Replace, new(newItem), new(oldItem), startingIndex, startingIndex);
        }

        public static NotifyCollectionChangedEventArgs<T> Replace(ReadOnlySpan<T> newItems, ReadOnlySpan<T> oldItems, int startingIndex)
        {
            return new NotifyCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Replace, newItems, oldItems, startingIndex, startingIndex);
        }

        public static NotifyCollectionChangedEventArgs<T> Move(in T changedItem, int newStartingIndex, int oldStartingIndex)
        {
            return new NotifyCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Move, new(changedItem), default, newStartingIndex, oldStartingIndex);
        }

        public static NotifyCollectionChangedEventArgs<T> Reset(ReadOnlySpan<T> newItems, ReadOnlySpan<T> oldItems)
        {
            return new NotifyCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Reset, newItems, oldItems);
        }
    }
}
