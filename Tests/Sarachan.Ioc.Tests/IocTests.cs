using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sarachan.Ioc.Tests
{
    [TestClass]
    public class IocTests
    {
        class ServiceSingleton : IDisposable
        {
            public bool Disposed { get; private set; }
            public Guid Id { get; } = Guid.NewGuid();

            public override string ToString()
            {
                return Id.ToString();
            }

            public void Dispose()
            {
                Disposed = true;
            }
        }

        class ServiceScoped : ServiceSingleton { }

        class ServiceTransient : ServiceSingleton { }

        class ServiceMerged : ServiceSingleton { }

        class ServiceMerged2 : ServiceSingleton { }

        private static IServiceProvider CreateBaseProvider()
        {
            var services = new ServiceCollection();
            services.AddMergedProvider();
            services.AddSingleton<IServiceProviderFactory<IServiceCollection>, DefaultServiceProviderFactory>();

            services.AddSingleton<ServiceSingleton>();
            services.AddScoped<ServiceScoped>();
            services.AddTransient<ServiceTransient>();

            return services.BuildServiceProvider();
        }

        private static IServiceScope CreateChildScope<T>(IServiceProvider provider) where T : class
        {
            return provider.CreateScope(services =>
            {
                services.AddSingleton<T>();
            });
        }

        [TestMethod]
        public void TestServiceCore()
        {
            var provider = CreateBaseProvider();

            var scope = CreateChildScope<ServiceMerged>(provider);
            var scope2 = CreateChildScope<ServiceMerged2>(scope.ServiceProvider);

            provider = scope2.ServiceProvider.GetRequiredService<IServiceProvider>();

            provider.GetRequiredService<ServiceSingleton>();
            provider.GetRequiredService<ServiceScoped>();
            provider.GetRequiredService<ServiceTransient>();
            provider.GetRequiredService<ServiceMerged>();
            provider.GetRequiredService<ServiceMerged2>();

            var isService = provider.GetRequiredService<IServiceProviderIsService>();
            Assert.IsTrue(isService.IsService(typeof(ServiceSingleton)));
            Assert.IsTrue(isService.IsService(typeof(ServiceMerged)));
            Assert.IsTrue(isService.IsService(typeof(ServiceMerged2)));
        }

        [TestMethod]
        public void TestGetService()
        {
            var provider = CreateBaseProvider();

            var singleton = provider.GetRequiredService<ServiceSingleton>();
            var scoped = provider.GetRequiredService<ServiceScoped>();

            Assert.AreNotEqual(provider.GetRequiredService<ServiceTransient>().Id, provider.GetRequiredService<ServiceTransient>().Id);

            var scope1 = CreateChildScope<ServiceMerged>(provider);

            var singleton1 = scope1.ServiceProvider.GetRequiredService<ServiceSingleton>();
            Assert.AreEqual(singleton.Id, singleton1.Id);

            var scoped1 = scope1.ServiceProvider.GetRequiredService<ServiceScoped>();
            Assert.AreNotEqual(scoped.Id, scoped1.Id);

            var merged1 = provider.GetService<ServiceMerged>();
            Assert.IsNull(merged1);

            merged1 = scope1.ServiceProvider.GetRequiredService<ServiceMerged>();

            var scope2 = CreateChildScope<ServiceMerged>(scope1.ServiceProvider);
            var merged2 = scope2.ServiceProvider.GetRequiredService<ServiceMerged>();
            Assert.AreNotEqual(merged1.Id, merged2.Id);

            var scope22 = CreateChildScope<ServiceMerged2>(scope2.ServiceProvider);
            scope22.ServiceProvider.GetRequiredService<ServiceMerged2>();
        }

        [TestMethod]
        public void TestDispose()
        {
            var rootProvider = CreateBaseProvider();
            var scope = CreateChildScope<ServiceMerged>(rootProvider);
            var scope2 = CreateChildScope<ServiceMerged2>(scope.ServiceProvider);

            var singleton = scope2.ServiceProvider.GetRequiredService<ServiceSingleton>();
            var scoped = scope2.ServiceProvider.GetRequiredService<ServiceScoped>();
            var merged2 = scope2.ServiceProvider.GetRequiredService<ServiceMerged2>();

            scope2.Dispose();

            Assert.IsFalse(singleton.Disposed);
            Assert.IsTrue(scoped.Disposed);
            Assert.IsTrue(merged2.Disposed);
        }

        class ViewModel
        {
            public ServiceSingleton ServiceSingleton { get; }
            public ServiceScoped ServiceScoped { get; }
            public ServiceTransient ServiceTransient { get; }
            public ServiceMerged ServiceMerged { get; }

            public ViewModel(ServiceSingleton serviceSingleton, ServiceScoped serviceScoped, ServiceTransient serviceTransient, ServiceMerged serviceMerged)
            {
                ServiceSingleton = serviceSingleton;
                ServiceScoped = serviceScoped;
                ServiceTransient = serviceTransient;
                ServiceMerged = serviceMerged;
            }
        }

        [TestMethod]
        public void TestCreateInstance()
        {
            var provider = CreateBaseProvider();
            var scope = CreateChildScope<ServiceMerged>(provider);

            var vm1 = ActivatorUtilities.CreateInstance<ViewModel>(scope.ServiceProvider);
            var vm2 = ActivatorUtilities.CreateInstance<ViewModel>(scope.ServiceProvider);

            Assert.AreEqual(vm1.ServiceSingleton.Id, vm2.ServiceSingleton.Id);
            Assert.AreEqual(vm1.ServiceScoped.Id, vm2.ServiceScoped.Id);
            Assert.AreNotEqual(vm1.ServiceTransient.Id, vm2.ServiceTransient.Id);
            Assert.AreEqual(vm1.ServiceMerged.Id, vm2.ServiceMerged.Id);
        }

        [TestMethod]
        public void TestGetComplexService()
        {
            var provider = CreateBaseProvider();
            var scope = CreateChildScope<ServiceMerged>(provider);

            var scope1 = CreateChildScope<ViewModel>(scope.ServiceProvider);

            scope1.ServiceProvider.GetRequiredService<ViewModel>();
        }
    }
}