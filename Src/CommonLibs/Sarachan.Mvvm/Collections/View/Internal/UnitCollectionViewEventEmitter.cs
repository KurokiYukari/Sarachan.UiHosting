namespace Sarachan.Mvvm.Collections.View.Internal
{
    class UnitCollectionViewEventEmitter<T> : CollectionView.IEventEmitter<T, T>
    {
        public static UnitCollectionViewEventEmitter<T> Instance { get; } = new();

        public void Emit(NotifyCollectionChangedEventArgs<T> e, NotifyCollectionChangedEventHandler<T> handler)
        {
            handler(this, e);
        }
    }
}
