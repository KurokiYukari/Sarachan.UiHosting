using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarachan.UiHosting.Mvvm.Disposable
{
    internal sealed class RelayDisposable : IDisposable
    {
        private readonly Action _disposeAction;

        private bool _disposed;

        public RelayDisposable(Action disposeAction)
        {
            _disposeAction = disposeAction;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposeAction();
            _disposed = true;
        }
    }
}
