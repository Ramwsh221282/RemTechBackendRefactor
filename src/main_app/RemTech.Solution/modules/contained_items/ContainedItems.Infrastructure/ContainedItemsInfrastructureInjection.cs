using System.Reflection;
using ContainedItems.Domain.Contracts;
using ContainedItems.Infrastructure.BackgroundServices;
using ContainedItems.Infrastructure.Producers;
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
            services.RegisterRepository();
            services.RegisterProducers();
            services.RegisterConsumers();
            services.RegisterBackgroundServices();
        }

        private void RegisterRepository()
        {
            services.AddScoped<IContainedItemsRepository, ContainedItemsRepository>();
        }

        private void RegisterBackgroundServices()
        {
            services.AddHostedService<PublishContainedItemsToAddBackgroundService>();
            services.AddHostedService<AggregatedConsumersHostedService>();
        }

        private void RegisterProducers()
        {
            services.AddSingleton<ItemPublishStrategyFactory>();
            services.AddSingleton<AddSparesProducer>();
            services.AddSingleton<AddVehiclesProducer>();
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