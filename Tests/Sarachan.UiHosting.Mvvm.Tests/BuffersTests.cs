using System.ComponentModel;
using System.Diagnostics.Metrics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarachan.Mvvm.Buffers;
using Sarachan.Mvvm.Events;

namespace Sarachan.Mvvm.Tests
{
    [TestClass]
    public class BuffersTests
    {
        [TestMethod]
        public void TestSpanOwner()
        {
            int count = 24;
            var numbers = GenerateNumbers(count).ToArray();

            using (var owner = SpanOwner.Allocate(GenerateNumbers(count)))
            {
                for (int i = 0; i < count; i++)
                {
                    Assert.AreEqual(numbers[i], owner.Span[i]);
                }
            }
        }

        private static IEnumerable<int> GenerateNumbers(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return i;
            }
        }
    }
}