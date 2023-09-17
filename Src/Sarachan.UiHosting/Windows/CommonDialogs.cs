using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace Sarachan.UiHosting.Windows
{
    public partial class ConfirmDialogViewModel : DialogViewModelBase
    {
        [RelayCommand]
        private Task Cancel()
        {
            Result = false;
            return CloseAsync();
        }
    }

    public partial class SimpleInformationDialog : DialogViewModelBase
    {
        [ObservableProperty]
        private LogLevel _informationType;
    }
}