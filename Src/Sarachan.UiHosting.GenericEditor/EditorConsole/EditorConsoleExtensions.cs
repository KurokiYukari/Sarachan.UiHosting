using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sarachan.UiHosting.GenericEditor.EditorConsole.Extensions
{
    public static class EditorConsoleExtensions
    {
        public static ILoggingBuilder AddEditorConsole(this ILoggingBuilder self)
        {
            self.Services.AddSingleton<ILoggerProvider, EditorConsoleLoggerProvider>()
                .AddSingleton<IEditorConsoleService, EditorConsoleService>();
            return self;
        }
    }
}
