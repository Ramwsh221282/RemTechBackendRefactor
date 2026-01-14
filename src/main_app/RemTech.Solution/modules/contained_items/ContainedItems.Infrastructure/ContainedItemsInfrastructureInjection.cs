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
            services.RegisterBackgroundServices();
        }

        private void RegisterRepository()
        {
            services.AddScoped<IContainedItemsRepository, ContainedItemsRepository>();
        }

        private void RegisterBackgroundServices()
        {
            services.AddHostedService<PublishContainedItemsToAddBackgroundService>();
        }

        private void RegisterProducers()
        {
            services.AddSingleton<ItemPublishStrategyFactory>();
            services.AddSingleton<AddSparesProducer>();
            services.AddSingleton<AddVehiclesProducer>();
        }
    }
}
