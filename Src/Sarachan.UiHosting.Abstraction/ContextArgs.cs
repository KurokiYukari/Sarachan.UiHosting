using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarachan.UiHosting
{
    public class ContextArgs : EventArgs
    {
        private Dictionary<object, object?>? _context;

        public void SetContext(object key, object? context)
        {
            _context ??= new Dictionary<object, object?>();
            _context[key] = context;
        }

        public void SetContext(ContextArgs other)
        {
            var context = other._context;
            if (context != null)
            {
                foreach (var (k, v) in context)
                {
                    SetContext(k, v);
                }
            }
        }

        public bool TryGetContext<T>(object key, [MaybeNullWhen(false)] out T context)
        {
            if (_context == null)
            {
                context = default;
                return false;
            }

            bool result = _context.TryGetValue(key, out var obj);
            if (result && obj is T targetObj)
            {
                context = targetObj;
                return true;
            }
            else
            {
                context = default;
                return false;
            }
        }
    }
}
