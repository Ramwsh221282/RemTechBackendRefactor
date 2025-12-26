using RemTech.SharedKernel.Configurations;

namespace ParsersControl.WebApi.Extensions;

public static class ConfigurationInitializing
{
    extension(IServiceCollection services)
    {
        public void AddInfrastructureConfiguration()
        {
            services.AddNpgSqlOptionsFromAppsettings();
            services.AddRabbitMqOptionsFromAppsettings();
        }
    }
}