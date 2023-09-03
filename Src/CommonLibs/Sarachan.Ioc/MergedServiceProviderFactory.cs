using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Sarachan.Ioc
{
    public class MergedServiceProviderBuilder
    {
        public IServiceProvider? SourceProvider { get; set; }

        public IServiceCollection Services { get; }

        public MergedServiceProviderBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }

    public interface IMergedServiceProviderFactory : IServiceProviderFactory<MergedServiceProviderBuilder>
    {
    }

    internal class MergedServiceProviderFactory : IMergedServiceProviderFactory
    {
        private readonly IServiceProviderFactory<IServiceCollection> _providerFactory;

        public MergedServiceProviderFactory(IServiceProviderFactory<IServiceCollection> providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public MergedServiceProviderBuilder CreateBuilder(IServiceCollection services)
        {
            return new MergedServiceProviderBuilder(services);
        }

        public IServiceProvider CreateServiceProvider(MergedServiceProviderBuilder containerBuilder)
        {
            var provider = containerBuilder.SourceProvider ?? throw new ArgumentException("SourceProvider not set", nameof(containerBuilder));
            var services = new MergedServiceCollection
            {
                containerBuilder.Services
            };

            var mergedProvider = MergedServiceProvider.Create(provider, _providerFactory, services);
            services.Initialize(mergedProvider);
            return mergedProvider;
        }
    }
} 
