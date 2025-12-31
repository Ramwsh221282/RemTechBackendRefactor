using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using Spares.Domain;
using Spares.Infrastructure;

namespace Spares.WebApi.Extensions;

public static class SparesWebApiDependencyInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterSparesModule(bool isDevelopment)
        {
            services.RegisterSharedInfrastructure(isDevelopment);
            services.AddSparesDomain();
            services.AddSparesInfrastructure();
        }

        private void RegisterSharedInfrastructure(bool isDevelopment)
        {
            services.RegisterLogging();
            if (isDevelopment)
            {
                services.AddNpgSqlOptionsFromAppsettings();
                services.AddRabbitMqOptionsFromAppsettings();
            }
            services.AddRabbitMq();
            services.AddPostgres();
        }
    }
}