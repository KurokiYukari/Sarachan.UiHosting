using System.Buffers;
using CommunityToolkit.Diagnostics;
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance.Buffers;

namespace Sarachan.UiHosting.Mvvm.Buffers
{
    public static class SpanOwner
    {
        public static SlicedSpanOwner<T> Slice<T>(SpanOwner<T> spanOwner, int start, int length)
        {
            if ((ulong)((long)(uint)start + (uint)length) > (uint)spanOwner.Length)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }
            return new SlicedSpanOwner<T>(spanOwner, start, length);
        }

        public static SpanOwner<T> Allocate<T>(T item, ArrayPool<T>? pool = null)
        {
            var owner = SpanOwner<T>.Allocate(1, pool ?? ArrayPool<T>.Shared);
            owner.Span[0] = item;
            return owner;
        }

        public static SlicedSpanOwner<T> Allocate<T>(IEnumerable<T> items, ArrayPool<T>? pool = null)
        {
            pool ??= ArrayPool<T>.Shared;
            SpanOwner<T> owner;

            int index = 0;
            if (items.TryGetNonEnumeratedCount(out var count))
            {
                owner = SpanOwner<T>.Allocate(count, pool);
                foreach (var item in items)
                {
                    owner.Span[index++] = item;
                }
            }
            else
            {
                owner = SpanOwner<T>.Allocate(16, pool);
                foreach (var item in items)
                {
                    if (index >= owner.Span.Length)
                    {
                        var size = owner.Span.Length * 2;
                        var oldOwner = owner;
                        owner = SpanOwner<T>.Allocate(size, pool);
                        oldOwner.Span.CopyTo(owner.Span);
                        oldOwner.Dispose();
                    }

                    owner.Span[index++] = item;
                }
            }

            return Slice(owner, 0, index);
        }
    }
}
