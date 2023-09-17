using Microsoft.Extensions.Logging;

namespace Sarachan.UiHosting.Windows.Extensions
{
    public static class DialogExtensions
    {
        //public static bool Confirm(this IWindowService self, IUiContext uiContext, object content, string? title = null)
        //{
        //    var vm = self.OpenDialog<ConfirmDialogViewModel>(uiContext, (vm, handle) =>
        //    {
        //        handle.Title = title ?? "Confirm";
        //        vm.Content = content;
        //    });
        //    return vm.Result.GetValueOrDefault(false);
        //}

        //public static void Information(this IWindowService self, IUiContext uiContext, object content, string? title)
        //{
        //    self.OpenDialog<SimpleInformationDialog>(uiContext, (vm, handle) =>
        //    {
        //        handle.Title = title ?? "Information";
        //        vm.InformationType = LogLevel.Information;
        //        vm.Content = content;
        //    });
        //}

        //public static void Warning(this IWindowService self, IUiContext uiContext, object content, string? title)
        //{
        //    self.OpenDialog<SimpleInformationDialog>(uiContext, (vm, handle) =>
        //    {
        //        handle.Title = title ?? "Warning";
        //        vm.InformationType = LogLevel.Warning;
        //        vm.Content = content;
        //    });
        //}
    }
}