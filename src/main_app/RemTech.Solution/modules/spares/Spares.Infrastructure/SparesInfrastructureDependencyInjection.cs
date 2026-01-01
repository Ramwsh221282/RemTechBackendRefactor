using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using Spares.Domain.Contracts;
using Spares.Domain.Models;
using Spares.Infrastructure.Consumers;
using Spares.Infrastructure.Migrations;
using Spares.Infrastructure.Repository;

namespace Spares.Infrastructure;

public static class SparesInfrastructureDependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddSparesInfrastructure()
        {
            services.RegisterConsumers();
            services.RegisterRepositories();
        }
        
        private void RegisterConsumers()
        {
            services.AddSingleton<IConsumer, AddSparesConsumer>();
            services.AddHostedService<AggregatedConsumersHostedService>();
        }

        private void RegisterRepositories()
        {
            services.AddMigrations([typeof(SparesSchemaMigration).Assembly]);
            services.AddScoped<ISparesRepository, SparesRepository>();
        }
    }
}