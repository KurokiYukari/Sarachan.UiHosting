namespace Sarachan.UiHosting.Mvvm.Collections
{
    public interface ICollectionView<T, TView> : IReadOnlyObservableCollection<TView>, IDisposable
    {
        IReadOnlyObservableCollection<T> Collection { get; }
    }
}
