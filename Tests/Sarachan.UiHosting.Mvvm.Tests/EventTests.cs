using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Sarachan.Mvvm.Disposable;
using Sarachan.Mvvm.Events;
using Sarachan.Mvvm.Events.Extensions;

namespace Sarachan.Mvvm.Tests
{
    [TestClass]
    public class EventTests
    {
        sealed class ViewModel : ObservableObject
        {
            private int _length;
            public int Length
            {
                get => _length;
                set => SetProperty(ref _length, value);
            }
        }

        sealed class Counter
        {
            public int Count { get; private set; }

            public void Tick() => Count++;

            public void Reset()
            {
                Count = 0;
            }
        }

        [TestMethod]
        public void TestPropertyChangedEvent()
        {
            var vm = new ViewModel();

            int countRef = 100;

            var counter = new Counter();

            var registration = CountPropertyChangedEvents(vm.CreatePropertyChangedEntry(), counter, countRef);
            vm.Length++;
            Assert.AreEqual(counter.Count, countRef);

            counter.Reset();
            registration.Dispose();
            registration = CountPropertyChangedEvents(vm.CreatePropertyChangedEntry().ToWeak(), counter, countRef);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            vm.Length++;

            Debug.WriteLine($"Count: {counter.Count}");
            Assert.AreNotEqual(counter.Count, countRef);

            counter.Reset();
            registration.Dispose();
        }

        private static IDisposable CountPropertyChangedEvents(IEventEntry<EventHandler<PropertyChangedEventArgs>> entry, Counter counter, int count)
        {
            IDisposable disposable = DisposableUtils.Unit;
            for (int i = 0; i < count; i++)
            {
                disposable = DisposableUtils.Combine(disposable, entry.Subscribe((sender, e) => counter.Tick()));
            }
            return disposable;
        } 
    }
}