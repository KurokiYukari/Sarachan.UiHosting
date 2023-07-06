namespace Sarachan.UiHosting.Windows
{
    public interface IWindowService
    {
        IUiContext UiContext { get; }

        IWindowHandle OpenWindow(object viewModel);

        void OpenDialog(object viewModel, Action<IWindowHandle> initializer);
    }
}