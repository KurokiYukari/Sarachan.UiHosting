using Microsoft.Extensions.Logging;

namespace Sarachan.UiHosting.GenericEditor.EditorConsole
{
    internal sealed class EditorConsoleLogger : ILogger
    {
        private readonly string _category;
        private readonly IEditorConsoleService _editorConsoleService;

        public EditorConsoleLogger(string category, IEditorConsoleService editorConsoleService)
        {
            _category = category;
            _editorConsoleService = editorConsoleService;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var entry = new EditorConsoleLogEntry(logLevel, eventId, exception, DateTime.Now, _category, formatter(state, exception), state);
            //_editorConsoleService.Entries.Add(entry);
        }
    }

    public sealed record EditorConsoleLogEntry(LogLevel LogLevel, EventId EventId, Exception? Exception, DateTime Time, string Category, string Message, object? State);
}
