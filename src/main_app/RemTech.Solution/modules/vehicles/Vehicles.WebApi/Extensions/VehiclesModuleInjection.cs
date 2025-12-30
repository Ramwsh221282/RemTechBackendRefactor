using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using RemTech.SharedKernel.NN;
using Vehicles.Domain;
using Vehicles.Infrastructure;

namespace Vehicles.WebApi.Extensions;

public static class VehiclesModuleInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterVehiclesModule(bool isDevelopment)
        {
            services.RegisterLogging();
            if (isDevelopment)
            {
                services.AddNpgSqlOptionsFromAppsettings();
                services.AddRabbitMqOptionsFromAppsettings();
                EmbeddingsProviderExtensions.RegisterFromAppsettings(services);
            }
            
            services.AddPostgres();
            services.AddRabbitMq();
            services.RegisterVehiclesModuleInfrastructure();
            services.RegisterVehiclesDomain();
            services.AddSingleton<EmbeddingsProvider>();
        }
    }
}