using Microsoft.Extensions.DependencyInjection.Extensions;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using RemTech.SharedKernel.NN;
using Spares.Domain.Contracts;
using Spares.Domain.Models;
using Spares.Infrastructure.BackgroundServices;
using Spares.Infrastructure.Consumers;
using Spares.Infrastructure.Migrations;
using Spares.Infrastructure.Queries.GetSpares;
using Spares.Infrastructure.Repository;

namespace Spares.WebApi.Extensions;

public static class SparesModuleInjection
{
    extension(IServiceCollection services)
    {
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
        }

        private void RegisterBackgroundServices()
        {
            services.AddHostedService<SparesEmbeddingUpdaterService>();
        }
        
        private void RegisterRepositories()
        {
            services.AddMigrations([typeof(SparesSchemaMigration).Assembly]);
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
        
        private void RegisterConsumers()
        {
            services.AddSingleton<IConsumer, AddSparesConsumer>();
            services.AddHostedService<AggregatedConsumersHostedService>();
        }
    }
}