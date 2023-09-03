namespace Sarachan.UiHosting
{
    public interface IUiContext
    {
        IServiceProvider Provider { get; }

        Task<TResult> InvokeAsync<TResult>(Func<UiContextInvokeArgs?, TResult> function, UiContextInvokeArgs? args);
    }

    public class UiContextInvokeArgs
    {
        private Dictionary<object, object?>? _context;

        public CancellationToken CancellationToken { get; }

        public UiContextInvokeArgs(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
        }

        public void SetContext(object key, object? context)
        {
            _context ??= new Dictionary<object, object?>();
            _context[key] = context;
        }

        public bool TryGetContext(object key, out object? context)
        {
            if (_context == null)
            {
                context = null;
                return false;
            }

            bool result = _context.TryGetValue(key, out context);
            return result;
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