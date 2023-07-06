namespace Sarachan.UiHosting
{
    public interface IFallbackExceptionHandler
    {
        bool HandleException(Exception exception);
    }
}