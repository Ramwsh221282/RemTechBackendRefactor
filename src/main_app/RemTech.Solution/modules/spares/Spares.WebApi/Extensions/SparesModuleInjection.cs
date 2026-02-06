using Microsoft.Extensions.DependencyInjection.Extensions;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using RemTech.SharedKernel.NN;
using Spares.Domain.Contracts;
using Spares.Domain.Features;
using Spares.Infrastructure.BackgroundServices;
using Spares.Infrastructure.Migrations;
using Spares.Infrastructure.Oems;
using Spares.Infrastructure.RabbitMq.Producers;
using Spares.Infrastructure.Repository;
using Spares.Infrastructure.Types;

namespace Spares.WebApi.Extensions;

/// <summary>
/// Расширения для внедрения модуля запчастей.
/// </summary>
public static class SparesModuleInjection
{
	extension(IServiceCollection services)
	{
		public void InjectSparesModule()
		{
			services.RegisterSparesInfrastructure();
		}

		public void RegisterSparesModule(bool isDevelopment)
		{
			services.RegisterSharedInfrastructure(isDevelopment);
			services.RegisterSparesInfrastructure();
		}

		private void RegisterSharedInfrastructure(bool isDevelopment)
		{
			services.RegisterLogging();
			if (isDevelopment)
			{
				services.AddMigrations([typeof(SparesSchemaMigration).Assembly]);
				services.AddOptions<NpgSqlOptions>().BindConfiguration(nameof(NpgSqlOptions));
				services.AddOptions<RabbitMqOptions>().BindConfiguration(nameof(RabbitMqOptions));
				services.AddOptions<EmbeddingsProviderOptions>().BindConfiguration(nameof(EmbeddingsProviderOptions));
			}

			services.TryAddSingleton<EmbeddingsProvider>();
			services.AddRabbitMq();
			services.AddPostgres();
		}

		public void RegisterSparesInfrastructure()
		{
			services.RegisterRepositories();
			services.RegisterRegionProvider();
			services.RegisterBackgroundServices();
			services.RegisterProducers();
		}

		private void RegisterBackgroundServices()
		{
			services.AddHostedService<SparesEmbeddingUpdaterService>();
		}

		private void RegisterRepositories()
		{
			services.AddScoped<ISparesRepository, SparesRepository>();
			services.AddScoped<ISpareOemsRepository, SparesOemRepository>();
			services.AddScoped<ISpareTypesRepository, SpareTypesRepository>();
		}

		private void RegisterRegionProvider()
		{
			services.AddScoped<ISpareAddressProvider, EmbeddingSearchAddressProvider>();
		}

		private void RegisterProducers()
		{
			services.AddSingleton<IOnSparesAddedEventPublisher, OnVehiclesAddedProducer>();
		}
	}
}
