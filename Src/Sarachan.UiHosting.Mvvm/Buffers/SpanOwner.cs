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
            Guard.IsInRangeFor(start, spanOwner.Span);
            Guard.HasSizeGreaterThanOrEqualTo(spanOwner.Span, start + length);
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

            if (index == 0)
            {
                return new SlicedSpanOwner<T>(owner, 0, 0);
            }
            else
            {
                return Slice(owner, 0, index);
            }
        }

        public static SpanOwner<T> Allocate<T>(IReadOnlyList<T> list, int index, int length, ArrayPool<T>? pool = null)
        {
            var owner = SpanOwner<T>.Allocate(length, pool ?? ArrayPool<T>.Shared);
            for (int i = 0; i < length; i++)
            {
                owner.Span[i] = list[i + index];
            }
            return owner;
        }
    }
}
