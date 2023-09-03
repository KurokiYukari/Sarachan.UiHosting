namespace Sarachan.UiHosting.Windows
{
    public interface IWindowService
    {
        IWindowHandle OpenWindow(IUiContext uiContext, object viewModel);

        void OpenDialog(IUiContext uiContext, object viewModel, Action<IWindowHandle> initializer);
    }
}