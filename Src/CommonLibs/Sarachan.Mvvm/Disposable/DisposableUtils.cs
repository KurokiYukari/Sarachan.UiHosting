using System.Diagnostics.CodeAnalysis;

namespace Sarachan.Mvvm.Disposable
{
    public static class DisposableUtils
    {
        sealed class UnitDisposable : IDisposable
        {
            public void Dispose() { }
        }

        public static IDisposable Unit { get; } = new UnitDisposable();

        public static IDisposable Create(Action action)
        {
            return new RelayDisposable(action);
        }

        public static IDisposable Combine(IDisposable? disposable1, IDisposable? disposable2)
        {
            CombinedDisposable combined;
            if (disposable1 is CombinedDisposable combined1)
            {
                combined1.AddDisposable(disposable2);
                combined = combined1;
            }
            else if (disposable2 is CombinedDisposable combined2)
            {
                combined2.AddDisposable(disposable1);
                combined = combined2;
            }
            else
            {
                combined = new CombinedDisposable();
                combined.AddDisposable(disposable1);
                combined.AddDisposable(disposable2);
            }

            return combined;
        }

        public static void IsNotDisposed([DoesNotReturnIf(true)] bool disposed, string? objectName = null)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(objectName);
            }
        }
    }
}
