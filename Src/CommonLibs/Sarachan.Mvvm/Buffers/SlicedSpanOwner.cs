using CommunityToolkit.HighPerformance.Buffers;

namespace Sarachan.Mvvm.Buffers
{
    public readonly ref struct SlicedSpanOwner<T>
    {
        private readonly SpanOwner<T> _spanOwner;

        private readonly int _start;
        public int Length { get; }

        public Span<T> Span => _spanOwner.Span[_start..Length];

        public SlicedSpanOwner(SpanOwner<T> spanOwner, int start, int length)
        {
            _spanOwner = spanOwner;
            _start = start;
            Length = length;
        }

        public void Dispose()
        {
            _spanOwner.Dispose();
        }
    }
}
