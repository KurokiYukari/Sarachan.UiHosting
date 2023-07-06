using Microsoft.Extensions.DependencyInjection;

namespace Sarachan.UiHosting.Windows.Extensions
{
    public static class WindowServiceExtensions
    {
        public static T OpenWindow<T>(this IWindowService self, Action<T>? initializer, out IWindowHandle handle)
        {
            var provider = self.UiContext.Provider;
            var viewModel = ActivatorUtilities.GetServiceOrCreateInstance<T>(provider)!;
            initializer?.Invoke(viewModel);
            handle = self.OpenWindow(viewModel);

            if (viewModel is IWindowViewModel window)
            {
                window.OnOpening(handle);
            }

            return viewModel;
        }

        public static T OpenWindow<T>(this IWindowService self, Action<T>? initializer = null)
        {
            return OpenWindow(self, initializer, out _);
        }

        public static T OpenDialog<T>(this IWindowService self, Action<T, IWindowHandle>? initializer)
        {
            var provider = self.UiContext.Provider;
            var viewModel = ActivatorUtilities.GetServiceOrCreateInstance<T>(provider)!;
            self.OpenDialog(viewModel, handle =>
            {
                initializer?.Invoke(viewModel, handle);
            });

            return viewModel;
        }
    }
}