using System;
using System.Collections;
using System.Collections.Specialized;
using Sarachan.Mvvm.Collections.View.Internal;
using Sarachan.Mvvm.Events;

namespace Sarachan.Mvvm.Collections.View
{
    public interface ICollectionView<T, TView> : IReadOnlyObservableList<TView>, IDisposable
    {
        IReadOnlyObservableCollection<T> Collection { get; }

        void Refresh();
    }

    public static class CollectionView
    {
        public interface IEventEmitter<T, TView>
        {
            void Emit(NotifyCollectionChangedEventArgs<T> e, NotifyCollectionChangedEventHandler<TView> handler);
        }

        public static ICollectionView<T, TView> BuildView<T, TView>(this IReadOnlyObservableCollection<T> self, 
            Func<IEventEmitter<T, T>, IEventEmitter<T, TView>> emitterBuilder)
        {
            var emitter = emitterBuilder(UnitCollectionViewEventEmitter<T>.Instance);
            var view = new CollectionView<T, TView>(self, null, emitter);
            view.Refresh();
            return view;
        }

        public static IEventEmitter<T, TView> Combine<T, TRelay, TView>(this IEventEmitter<T, TRelay> self, IEventEmitter<TRelay, TView> other)
        {
            return new CombinedCollectionViewEventEmitter<T, TRelay, TView>(self, other);
        }

        public static IEventEmitter<T, TToView> Select<T, TFromView, TToView>(this IEventEmitter<T, TFromView> self, Func<TFromView, TToView> selector)
        {
            var selectEmitter = new SelectCollectionViewEventEmitter<TFromView, TToView>(selector);
            return self.Combine(selectEmitter);
        }

        public static IEventEmitter<T, TView> Where<T, TView>(this IEventEmitter<T, TView> self, Predicate<TView> predicate)
        {
            var whereEmitter = new WhereCollectionViewEventEmitter<TView>(predicate);
            return self.Combine(whereEmitter);
        }
    }
}
