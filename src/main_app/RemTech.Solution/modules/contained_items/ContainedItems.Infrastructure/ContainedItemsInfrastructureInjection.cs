using ContainedItems.Domain.Contracts;
using ContainedItems.Infrastructure.BackgroundServices;
using ContainedItems.Infrastructure.Producers;
using ContainedItems.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ContainedItems.Infrastructure;

/// <summary>
/// Расширения для регистрации инфраструктуры содержащихся элементов в контейнере служб.
/// </summary>
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
            services.AddHostedService<MainPageStatsCacheInvalidationBackgroundService>();
            services.AddHostedService<MainPageLastAddedItemsInvalidationBackgroundService>();
        }

		private void RegisterProducers()
		{
			services.AddSingleton<ItemPublishStrategyFactory>();
			services.AddSingleton<AddSparesProducer>();
			services.AddSingleton<AddVehiclesProducer>();
		}
	}
}
