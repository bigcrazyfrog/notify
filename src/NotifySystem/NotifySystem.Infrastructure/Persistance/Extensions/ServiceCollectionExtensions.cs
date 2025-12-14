using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NotifySystem.Core.Domain.SharedKernel.Base;
using NotifySystem.Core.Domain.SharedKernel.Storage;

namespace NotifySystem.Infrastructure.Persistance.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static RepositoryRegistrar<TRepository> RegisterRepository<TRepository, TReadOnlyRepository, TRepositoryImpl>(
            this IServiceCollection serviceCollection) 
            where TRepository : class
            where TReadOnlyRepository : class
            where TRepositoryImpl : class, TRepository
        {
            serviceCollection.AddTransient<TRepositoryImpl>();
            serviceCollection.AddTransient<TRepository, TRepositoryImpl>();
            serviceCollection.AddTransient<TReadOnlyRepository>(provider =>
            {
                var impl = provider.GetRequiredService<TRepositoryImpl>();
                var readOnlyProperty = impl.GetType().GetProperty("ReadOnly");
                readOnlyProperty?.SetValue(impl, true);
                
                return (TReadOnlyRepository)(object)impl;
            });
            
            return new RepositoryRegistrar<TRepository>(serviceCollection);
        }

        public static RepositoryRegistrar<TRepository> RegisterRepository<TRepository, TRepositoryImpl>(
            this IServiceCollection serviceCollection)
            where TRepository : class
            where TRepositoryImpl : class, TRepository
        {
            serviceCollection.AddTransient<TRepositoryImpl>();
            serviceCollection.AddTransient<TRepository, TRepositoryImpl>();

            return new RepositoryRegistrar<TRepository>(serviceCollection);
        }
    }

    public class RepositoryRegistrar<TRepository> where TRepository : class
    {
        private readonly IServiceCollection _serviceCollection;
        public RepositoryRegistrar(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public RepositoryRegistrar<TRepository> AddDecorator<TDecorator>()
            where TDecorator : class, TRepository
        {
            _serviceCollection.Replace(ServiceDescriptor.Transient<TRepository, TDecorator>());
            return this;
        }

        public RepositoryRegistrar<TRepository> AddDecorator<TDecorator>(Func<IServiceProvider, TDecorator> factory)
            where TDecorator : class, TRepository
        {
            _serviceCollection.AddTransient<TRepository, TDecorator>(factory);
            return this;
        }
    }
}
