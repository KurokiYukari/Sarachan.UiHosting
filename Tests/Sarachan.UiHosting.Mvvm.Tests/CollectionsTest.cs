using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarachan.Mvvm.Collections;
using Sarachan.Mvvm.Collections.Extensions;
using Sarachan.Mvvm.Collections.View;
using static Sarachan.Mvvm.Collections.View.CollectionView;

namespace Sarachan.Mvvm.Tests
{
    [TestClass]
    public class CollectionsTest
    {
        private void RunTestTemplate(Func<IEventEmitter<int, int>, IEventEmitter<int, int>> emitterBuilder,
            Func<IEnumerable<int>, IEnumerable<int>> refCollectionBuilder)
        {
            const int RANDOM_COUNT = 100;
            static int GetRandomItem()
            {
                return Random.Shared.Next(0, 100);
            }
            static void GetRandomItems(Span<int> newItems)
            {
                for (int i = 0; i < newItems.Length; i++)
                {
                    newItems[i] = GetRandomItem();
                }
            }

            var list = new ObservableList<int>();

            for (int i = 0; i < RANDOM_COUNT * 2; i++)
            {
                list.Add(GetRandomItem());
            }

            var view = list.BuildView(emitterBuilder);

            void AssertSequenceEqual()
            {
                Assert.IsTrue(view.SequenceEqual(refCollectionBuilder(list)));
            }

            // Init
            AssertSequenceEqual();

            // Refresh
            Span<int> resetItems = stackalloc int[RANDOM_COUNT * 2];
            for (int i = 0; i < resetItems.Length; i++)
            {
                resetItems[i] = GetRandomItem();
            }
            list.Reset(resetItems);

            AssertSequenceEqual();

            // Insert
            Span<int> tempItems = stackalloc int[2];
            for (int i = 0; i < RANDOM_COUNT; i++)
            {
                GetRandomItems(tempItems);
                list.Insert(Random.Shared.Next(0, list.Count), tempItems);
            }
            GetRandomItems(tempItems);
            list.Insert(0, tempItems);
            GetRandomItems(tempItems);
            list.Add(tempItems);

            AssertSequenceEqual();

            // Remove
            for (int i = 0; i < RANDOM_COUNT; i++)
            {
                int index = Random.Shared.Next(0, list.Count - 2);
                //tempItems[0] = list[index];
                //tempItems[1] = list[index + 1];
                list.RemoveAt(index, 2);
                AssertSequenceEqual();
            }

            // Replace
            for (int i = 0; i < RANDOM_COUNT; i++)
            {
                var index = Random.Shared.Next(0, list.Count - 1);
                var randomItem = GetRandomItem();
                list[index] = randomItem;
                AssertSequenceEqual();
            }

            // Move
            for (int i = 0; i < RANDOM_COUNT; i++)
            {
                list.Move(Random.Shared.Next(0, list.Count - 1), Random.Shared.Next(0, list.Count - 1));
            }

            AssertSequenceEqual();
        }

        [TestMethod]
        public void TestSelect()
        {
            static int Selector(int x) => x * 2;
            RunTestTemplate(emitter => emitter.Select(Selector),
                source => source.Select(Selector));
        }

        [TestMethod]
        public void TestOrderBy()
        {
            RunTestTemplate(emitter => emitter.OrderBy(),
                source => source.OrderBy(x => x));
        }

        [TestMethod]
        public void TestReverse()
        {
            RunTestTemplate(emitter => emitter.Reverse(),
                source => source.Reverse());
        }

        [TestMethod]
        public void TestWhere()
        {
            static bool Predicate(int x)
            {
                return x % 2 == 0;
            }
            RunTestTemplate(emitter => emitter.Where(Predicate),
                source => source.Where(Predicate));
        }

        [TestMethod]
        public void TestOrderByAndReverse()
        {
            RunTestTemplate(emitter => emitter.OrderBy().Reverse(),
                source => source.OrderBy(x => x).Reverse());
        }

        [TestMethod]
        public void TestWhereAndOrderBy()
        {
            static bool Predicate(int x)
            {
                return x % 2 == 0;
            }
            RunTestTemplate(emitter => emitter.Where(Predicate).OrderBy(),
                source => source.Where(Predicate).OrderBy(x => x));
        }
    }
}