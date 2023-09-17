using CommunityToolkit.Mvvm.ComponentModel;

namespace Sarachan.UiHosting.Windows
{
    public sealed partial class GenericWindowHandle : ObservableObject
        //, IWindowHandle
    {
        [ObservableProperty]
        private string _title = string.Empty;

        private readonly Action _closeAction;

        public GenericWindowHandle(Action closeAction)
        {
            _closeAction = closeAction;
        }

        public void Close()
        {
            _closeAction();
        }
    }
}