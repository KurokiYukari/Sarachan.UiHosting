using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sarachan.UiHosting.Navigation;

namespace Sarachan.UiHosting.Windows
{
    public abstract partial class DialogViewModelBase : WindowViewModel
    {
        public bool? Result { get; protected set; }

        [ObservableProperty]
        private object? _content;

        public Task CloseAsync()
        {
            var args = new NavigationArgs.Close(Result);
            OnSettingCloseArgs(args);
            return CloseAsync(args);
        }

        protected virtual void OnSettingCloseArgs(NavigationArgs.Close args)
        {
        }

        [RelayCommand]
        private Task Confirm()
        {
            OnConfirm();
            Result = true;
            return CloseAsync();
        }
         
        protected virtual void OnConfirm()
        {

        }
    }
}