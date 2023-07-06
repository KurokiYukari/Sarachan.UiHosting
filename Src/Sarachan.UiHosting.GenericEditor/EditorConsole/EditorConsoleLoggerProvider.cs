using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sarachan.UiHosting.GenericEditor.EditorConsole
{
    [ProviderAlias("EditorConsole")]
    public sealed class EditorConsoleLoggerProvider : ILoggerProvider
    {
        private readonly IEditorConsoleService _editorConsoleService;

        private readonly ConcurrentDictionary<string, ILogger> _cache = new();

        public EditorConsoleLoggerProvider(IEditorConsoleService editorConsoleService)
        {
            _editorConsoleService = editorConsoleService;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _cache.GetOrAdd(categoryName, name => new EditorConsoleLogger(name, _editorConsoleService));
        }

        public void Dispose()
        {
            _cache.Clear();
        }
    }
}
