using Microsoft.Extensions.DependencyInjection;

namespace Sarachan.UiHosting.Navigation
{
    public static class NavigationServiceExtensions
    {
        public static async Task<T> NavigateAsync<T>(this INavigationService self, IUiContext uiContext, NavigationArgs.Navigate? args)
        {
            var provider = uiContext.Provider;
            var viewModel = ActivatorUtilities.GetServiceOrCreateInstance<T>(provider)!;

            args ??= new NavigationArgs.Navigate();
            self.Navigate(uiContext, viewModel, args);

            await Task.WhenAll(args.EventTasks);
            return viewModel;
        }

        //public static T OpenWindow<T>(this IWindowService self, IUiContext uiContext, Action<T>? initializer = null)
        //{
        //    return OpenWindow(self, uiContext, initializer, out _);
        //}

        //public static T OpenDialog<T>(this IWindowService self, IUiContext uiContext, Action<T, IWindowHandle>? initializer)
        //{
        //    var provider = uiContext.Provider;
        //    var viewModel = ActivatorUtilities.GetServiceOrCreateInstance<T>(provider)!;
        //    self.OpenDialog(uiContext, viewModel, handle =>
        //    {  
        //        initializer?.Invoke(viewModel, handle);
        //    });

        //    return viewModel;
        //}
    }
}