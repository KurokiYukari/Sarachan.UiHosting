﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarachan.Mvvm.Collections;
using Sarachan.Mvvm.Collections.Extensions;
using Sarachan.Mvvm.Collections.View;

namespace Sarachan.Mvvm.Tests
{
    [TestClass]
    public class CollectionsTest
    {
        private static (ObservableList<int>, ICollectionView<int, int>) CreateSampleData()
        {
            var list = new ObservableList<int>()
            {
                0, 1, 2, 3, 4, 5, 6,
            };

            //var view = list.CreateSelectView(i => i)
            //    .CreateWhereView(i => i % 2 == 0);

            var view = list.BuildView(emitter =>
                emitter.Select(i => i)
                    .Where(i => i % 2 == 0));

            return (list, view);
        }

        private static void AssertSampleData(ObservableList<int> list,
            ICollectionView<int, int> view,
            IEnumerable<int> data)
        {
            Assert.IsTrue(list.SequenceEqual(data));
            Assert.IsTrue(view.SequenceEqual(data.Where(i => i % 2 == 0)));
        }

        [TestMethod]
        public void TestMove()
        {
            var (list, view) = CreateSampleData();

            list.Move(0, 6);
            AssertSampleData(list, view, new[] { 1, 2, 3, 4, 5, 6, 0 });

            list.Move(6, 0);
            AssertSampleData(list, view, new[] { 0, 1, 2, 3, 4, 5, 6 });

            list.Move(0, 5);
            AssertSampleData(list, view, new[] { 1, 2, 3, 4, 5, 0, 6 });

            list.Move(5, 0);
            AssertSampleData(list, view, new[] { 0, 1, 2, 3, 4, 5, 6 });

            list.Move(1, 6);
            AssertSampleData(list, view, new[] { 0, 2, 3, 4, 5, 6, 1 });

            list.Move(6, 1);
            AssertSampleData(list, view, new[] {0, 1, 2, 3, 4, 5, 6 });
        }

        [TestMethod]
        public void TestAddAndRemove()
        {
            var (list, view) = CreateSampleData();

            list.Add(7);
            AssertSampleData(list, view, new[] { 0, 1, 2, 3, 4, 5, 6, 7 });

            list.Remove(7);
            AssertSampleData(list, view, new[] { 0, 1, 2, 3, 4, 5, 6 });

            list.Add(8);
            AssertSampleData(list, view, new[] { 0, 1, 2, 3, 4, 5, 6, 8 });

            list.Remove(8);
            AssertSampleData(list, view, new[] { 0, 1, 2, 3, 4, 5, 6 });

            list.Insert(2, 8);
            AssertSampleData(list, view, new[] { 0, 1, 8, 2, 3, 4, 5, 6 });

            list.Remove(8);
            AssertSampleData(list, view, new[] { 0, 1, 2, 3, 4, 5, 6 });
        }

        [TestMethod]
        public void TestReplace()
        {
            var (list, view) = CreateSampleData();

            list[0] = 1;
            AssertSampleData(list, view, new[] { 1, 1, 2, 3, 4, 5, 6 });

            list[1] = 2;
            AssertSampleData(list, view, new[] { 1, 2, 2, 3, 4, 5, 6 });
        }
    }
}