using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace RemTechAvitoVehiclesParser;

public static class InfrastrucuteInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterInfrastructureDependencies(bool isDevelopment)
        {
            services.AddMigrations([typeof(InfrastrucuteInjection).Assembly]);
            services.AddPostgres();
            services.AddRabbitMq();
        }
    }
}
