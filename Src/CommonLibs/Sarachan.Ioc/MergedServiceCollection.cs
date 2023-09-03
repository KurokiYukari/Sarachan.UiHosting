using System.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace Sarachan.Ioc
{
    sealed class MergedServiceCollection : IServiceCollection
    {
        private IServiceProvider? _mergedProvider;
        public IServiceProvider MergedProvider => _mergedProvider 
            ?? throw new InvalidOperationException($"{nameof(MergedServiceCollection)} has not been initialized");

        private readonly List<ServiceDescriptor> _descriptors = new();
        private bool _isReadOnly;

        /// <inheritdoc />
        public int Count => _descriptors.Count;

        /// <inheritdoc />
        public bool IsReadOnly => _isReadOnly;

        /// <inheritdoc />
        public ServiceDescriptor this[int index]
        {
            get
            {
                return _descriptors[index];
            }
            set
            {
                CheckReadOnly();
                _descriptors[index] = value;
            }
        }

        public void Initialize(IServiceProvider provider)
        {
            if (_mergedProvider != null)
            {
                throw new InvalidOperationException($"{nameof(MergedServiceCollection)} has been initialized");
            }

            _mergedProvider = provider;
        }

        /// <inheritdoc />
        public void Clear()
        {
            CheckReadOnly();
            _descriptors.Clear();
        }

        /// <inheritdoc />
        public bool Contains(ServiceDescriptor item)
        {
            return _descriptors.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            _descriptors.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(ServiceDescriptor item)
        {
            CheckReadOnly();
            return _descriptors.Remove(item);
        }

        /// <inheritdoc />
        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
            return _descriptors.GetEnumerator();
        }

        /// <inheritdoc />
        public int IndexOf(ServiceDescriptor item)
        {
            return _descriptors.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, ServiceDescriptor item)
        {
            CheckReadOnly();

            var descriptor = item;
            if (item.ImplementationFactory != null)
            {
                descriptor = new ServiceDescriptor(item.ServiceType, _ =>
                {
                    var provider = MergedProvider;
                    return item.ImplementationFactory(provider);
                }, item.Lifetime);
            }
            else if (item.ImplementationInstance == null)
            {
                descriptor = new ServiceDescriptor(item.ServiceType, _ =>
                {
                    var provider = MergedProvider;
                    return ActivatorUtilities.CreateInstance(provider, item.ImplementationType ?? item.ServiceType);
                }, item.Lifetime);
            }

            _descriptors.Insert(index, descriptor);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            CheckReadOnly();
            _descriptors.RemoveAt(index);
        }

        /// <summary>
        /// Makes this collection read-only.
        /// </summary>
        /// <remarks>
        /// After the collection is marked as read-only, any further attempt to modify it throws an <see cref="InvalidOperationException" />.
        /// </remarks>
        public void MakeReadOnly()
        {
            _isReadOnly = true;
        }

        private void CheckReadOnly()
        {
            if (_isReadOnly)
            {
                ThrowReadOnlyException();
            }
        }

        private static void ThrowReadOnlyException() =>
            throw new InvalidOperationException("Service Collection is ReadOnly");

        void ICollection<ServiceDescriptor>.Add(ServiceDescriptor item)
        {
            Insert(Count, item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
