using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sarachan.UiHosting.Mvvm.Collections;

namespace Sarachan.UiHosting.GenericEditor.EditorConsole
{
    public interface IEditorConsoleService
    {
        //INotifyCollection<EditorConsoleLogEntry> Entries { get; }
    }

    internal class EditorConsoleService : IEditorConsoleService
    {
        //private readonly NotifyCollection<EditorConsoleLogEntry> _entries = new();
        //public INotifyCollection<EditorConsoleLogEntry> Entries => _entries;
    }
}
