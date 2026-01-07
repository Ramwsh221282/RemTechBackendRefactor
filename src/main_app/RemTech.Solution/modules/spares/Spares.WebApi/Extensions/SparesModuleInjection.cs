using Microsoft.Extensions.DependencyInjection.Extensions;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using RemTech.SharedKernel.NN;
using Spares.Domain.Contracts;
using Spares.Domain.Features;
using Spares.Domain.Models;
using Spares.Infrastructure.BackgroundServices;
using Spares.Infrastructure.Migrations;
using Spares.Infrastructure.Queries.GetSpares;
using Spares.Infrastructure.RabbitMq.Consumers;
using Spares.Infrastructure.RabbitMq.Producers;
using Spares.Infrastructure.Repository;

namespace Spares.WebApi.Extensions;

public static class SparesModuleInjection
{
    extension(IServiceCollection services)
    {
        public void InjectSparesModule()
        {
            services.RegisterSparesDomain();
            services.RegisterSparesInfrastructure();
        }
        
        public void RegisterSparesModule(bool isDevelopment)
        {
            services.RegisterSharedInfrastructure(isDevelopment);
            services.RegisterSparesDomain();
            services.RegisterSparesInfrastructure();
        }

        private void RegisterSharedInfrastructure(bool isDevelopment)
        {
            services.RegisterLogging();
            if (isDevelopment)
            {
                services.AddMigrations([typeof(SparesSchemaMigration).Assembly]);
                services.AddNpgSqlOptionsFromAppsettings();
                services.AddRabbitMqOptionsFromAppsettings();
                services.AddOptions<EmbeddingsProviderOptions>().BindConfiguration(nameof(EmbeddingsProviderOptions));
                services.AddOptions<GetSparesThresholdConstants>().BindConfiguration(nameof(GetSparesThresholdConstants));
            }
            
            services.TryAddSingleton<EmbeddingsProvider>();
            services.AddRabbitMq();
            services.AddPostgres();
        }

        private void RegisterSparesDomain()
        {
            new HandlersRegistrator(services)
                .FromAssemblies([typeof(Spare).Assembly])
                .RequireRegistrationOf(typeof(ICommandHandler<,>))
                .RequireRegistrationOf(typeof(IEventTransporter<,>))
                .AlsoAddValidators()
                .AlsoAddDecorators()
                .AlsoUseDecorators()
                .Invoke();
        }
        
        private void RegisterSparesInfrastructure()
        {
            services.RegisterRepositories();
            services.RegisterRegionProvider();
            services.RegisterConsumers();
            services.RegisterBackgroundServices();
            services.RegisterQueryHandlers();
            services.RegisterProducers();
        }

        private void RegisterBackgroundServices()
        {
            services.AddHostedService<SparesEmbeddingUpdaterService>();
        }
        
        private void RegisterRepositories()
        {
            services.AddScoped<ISparesRepository, SparesRepository>();
        }

        private void RegisterRegionProvider()
        {
            services.AddScoped<ISpareAddressProvider, EmbeddingSearchAddressProvider>();
        }

        private void RegisterQueryHandlers()
        {
            new HandlersRegistrator(services)
                .FromAssemblies([typeof(GetSparesQuery).Assembly])
                .RequireRegistrationOf(typeof(IQueryHandler<,>))
                .Invoke();
        }
        
        private void RegisterProducers()
        {
            services.AddSingleton<IOnSparesAddedEventPublisher, OnVehiclesAddedProducer>();
        }
        
        private void RegisterConsumers()
        {
            services.AddSingleton<IConsumer, AddSparesConsumer>();
            services.AddHostedService<AggregatedConsumersHostedService>();
        }
    }
}