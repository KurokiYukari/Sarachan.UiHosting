namespace Sarachan.UiHosting.Mvvm.Disposable
{
    internal sealed class CombinedDisposable : IDisposable
    {
        private bool _disposed;

        private readonly HashSet<IDisposable> _disposables = new();

        public void AddDisposable(IDisposable? disposable)
        {
            DisposableUtils.IsNotDisposed(_disposed, nameof(CombinedDisposable));

            if (disposable == null)
            {
                return;
            }

            _disposables.Add(disposable);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            foreach (var item in _disposables)
            {
                item.Dispose();
            }
            _disposables.Clear();
            _disposed = true;
        }
    }
}
