using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Sarachan.UiHosting.Navigation;

namespace Sarachan.UiHosting.Windows
{
    public partial class WindowViewModel : ObservableObject, INavigationTarget
    {
        public event EventHandler<NavigationArgs.Close>? CloseRequested;

        void INavigationTarget.OnNavigated(NavigationArgs.Navigate args) 
        {
            OnNavigated(args);
        }

        protected virtual void OnNavigated(NavigationArgs.Navigate args)
        {
        }

        public Task CloseAsync(NavigationArgs.Close args)
        {
            RequestClose(args);
            return Task.WhenAll(args.EventTasks);
        }

        protected virtual void RequestClose(NavigationArgs.Close args)
        {
            CloseRequested?.Invoke(this, args);
        }

        ValueTask<bool> INavigationTarget.OnClosing(NavigationArgs.Close args)
        {
            return OnClosing(args);
        }

        protected virtual ValueTask<bool> OnClosing(NavigationArgs.Close args)
        {
            return ValueTask.FromResult(true);
        }

        void INavigationTarget.OnClosed(NavigationArgs.Close args)
        {
            OnClosed(args);
        }

        protected virtual void OnClosed(NavigationArgs.Close args)
        {
        }
    }
}