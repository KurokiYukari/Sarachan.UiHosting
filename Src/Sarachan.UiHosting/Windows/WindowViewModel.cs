using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Sarachan.UiHosting.Windows
{
    public partial class WindowViewModel : ObservableObject, IWindowViewModel
    {
        public IWindowHandle? WindowHandle { get; private set; }

        void IWindowViewModel.OnOpening(IWindowHandle handle)
        {
            Guard.IsNull(WindowHandle);
            WindowHandle = handle;
            OnOpening();
        }

        protected virtual void OnOpening()
        {
        }

        public void Close()
        {
            var handle = WindowHandle;
            Guard.IsNotNull(handle);

            if (OnClosing())
            {
                handle.Close();
                OnClosed();
            }
        }

        bool IWindowViewModel.OnClosing()
        {
            return OnClosing();
        }

        protected virtual bool OnClosing() => true;

        void IWindowViewModel.OnClosed()
        {
            OnClosed();
            WindowHandle = null;
        }

        protected virtual void OnClosed()
        {
        }
    }
}