using System.Reflection;
using ContainedItems.Domain.Contracts;
using ContainedItems.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ContainedItems.Infrastructure;

public static class ContainedItemsInfrastructureInjection
{
    extension(IServiceCollection services)
    {
        public void AddContainedItemsInfrastructure()
        {
            services.RegisterMigrations();
            services.RegisterRepository();
            services.RegisterConsumers();
        }

        private void RegisterMigrations()
        {
            services.AddMigrations([typeof(ContainedItemsInfrastructureInjection).Assembly]);;
        }

        private void RegisterRepository()
        {
            services.AddScoped<IContainedItemsRepository, ContainedItemsRepository>();
        }

        private void RegisterConsumers()
        {
            Assembly assembly = typeof(ContainedItemsInfrastructureInjection).Assembly;
            services.Scan(x => x.FromAssemblies(assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(IConsumer)))
                .AsSelfWithInterfaces()
                .WithTransientLifetime());
        }
    }
}