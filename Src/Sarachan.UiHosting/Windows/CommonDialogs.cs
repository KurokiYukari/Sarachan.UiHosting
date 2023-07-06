using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace Sarachan.UiHosting.Windows
{
    public partial class ConfirmDialogViewModel : DialogViewModelBase
    {
        public bool? Result { get; private set; }

        protected override void OnConfirm()
        {
            base.OnConfirm();
            Result = true;
        }

        [RelayCommand]
        private void Cancel()
        {
            Result = false;
            Close();
        }
    }

    public partial class SimpleInformationDialog : DialogViewModelBase
    {
        [ObservableProperty]
        private LogLevel _informationType;
    }
}