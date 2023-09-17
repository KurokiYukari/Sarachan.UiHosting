namespace Sarachan.UiHosting.Wpf.Extensions
{
    public class WpfUiOptions : UiOptions
    {
        public string WpfAppThreadName { get; set; } = "WpfApp";

        public string WpfThreadNameKey { get; set; } = "Wpf_ThreadName";

        public string ContextDispatcherPriorityKey { get; set; } = "Wpf_DispatcherPriority";
    }
}
