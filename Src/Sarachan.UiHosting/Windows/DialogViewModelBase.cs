using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Sarachan.UiHosting.Windows
{
    public abstract partial class DialogViewModelBase : WindowViewModel
    {
        [ObservableProperty]
        private object? _content;

        [RelayCommand]
        private void Confirm()
        {
            OnConfirm();
            Close();
        }

        protected virtual void OnConfirm()
        {

        }
    }
}