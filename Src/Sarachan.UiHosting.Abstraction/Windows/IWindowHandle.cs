namespace Sarachan.UiHosting.Windows
{
    public interface IWindowHandle
    {
        string Title { get; set; }

        void Close();
    }
}