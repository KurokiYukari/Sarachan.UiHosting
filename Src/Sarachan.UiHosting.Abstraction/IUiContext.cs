namespace Sarachan.UiHosting
{
    public interface IUiContext : IDisposable
    {
        IServiceProvider Provider { get; }

        Task<TResult> InvokeAsync<TResult>(Func<UiContextInvokeArgs?, TResult> function, UiContextInvokeArgs? args);
    }

    public interface IUiContextFactory
    {
        ValueTask<IUiContext> StartNew(UiContextStartNewArgs args);
    }

    public class UiContextStartNewArgs : ContextArgs
    {
    }

    public class UiContextInvokeArgs : ContextArgs
    {
        public CancellationToken CancellationToken { get; }

        public UiContextInvokeArgs(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
        }
    }

    public static class UiContextExtensions
    {
        public static Task<TResult> InvokeAsync<TResult>(this IUiContext self, Func<TResult> function)
        {
            return self.InvokeAsync(_ => function(), null);
        }

        public static async Task InvokeAsync(this IUiContext self, Action<UiContextInvokeArgs?> action, UiContextInvokeArgs? args)
        {
            var task = self.InvokeAsync(args =>
            {
                action(args);
                return (object?)null;
            }, args);
            await task;
        }

        public static Task InvokeAsync(this IUiContext self, Action action)
        {
            return self.InvokeAsync(_ => action(), null);
        }
    }
}