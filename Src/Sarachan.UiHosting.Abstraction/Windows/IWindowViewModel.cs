namespace Sarachan.UiHosting.Windows
{
    public interface IWindowViewModel
    {
        void OnOpening(IWindowHandle handle);

        bool OnClosing();

        void OnClosed();
    }
}