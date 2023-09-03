using Microsoft.Extensions.DependencyInjection;

namespace Sarachan.UiHosting.Windows.Extensions
{
    public static class WindowServiceExtensions
    {
        public static T OpenWindow<T>(this IWindowService self, IUiContext uiContext, Action<T>? initializer, out IWindowHandle handle)
        {
            var provider = uiContext.Provider;
            var viewModel = ActivatorUtilities.GetServiceOrCreateInstance<T>(provider)!;
            initializer?.Invoke(viewModel);
            handle = self.OpenWindow(uiContext, viewModel);

            if (viewModel is IWindowViewModel window)
            {
                window.OnOpening(handle);
            }

            return viewModel;
        }

        public static T OpenWindow<T>(this IWindowService self, IUiContext uiContext, Action<T>? initializer = null)
        {
            return OpenWindow(self, uiContext, initializer, out _);
        }

        public static T OpenDialog<T>(this IWindowService self, IUiContext uiContext, Action<T, IWindowHandle>? initializer)
        {
            var provider = uiContext.Provider;
            var viewModel = ActivatorUtilities.GetServiceOrCreateInstance<T>(provider)!;
            self.OpenDialog(uiContext, viewModel, handle =>
            {  
                initializer?.Invoke(viewModel, handle);
            });

            return viewModel;
        }
    }
}