namespace Sarachan.UiHosting
{
    public interface IUiContext
    {
        IServiceProvider Provider { get; }

        void Invoke(Action<object> action, object state);
    }
}