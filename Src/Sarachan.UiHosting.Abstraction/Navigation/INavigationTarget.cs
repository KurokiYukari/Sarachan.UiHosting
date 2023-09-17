namespace Sarachan.UiHosting.Navigation
{
    public interface INavigationTarget
    {
        event EventHandler<NavigationArgs.Close> CloseRequested;

        void OnNavigated(NavigationArgs.Navigate args);

        ValueTask<bool> OnClosing(NavigationArgs.Close args);

        void OnClosed(NavigationArgs.Close args);
    }
}