using Microsoft.Extensions.DependencyInjection;

namespace RemTech.SharedKernel.Configurations;

public static class RabbitMqOptionsExtensions
{
    extension(IServiceCollection services)
    {
        public void AddRabbitMqOptionsFromAppsettings(string sectionName = nameof(RabbitMqOptions)) =>
            services.AddOptions<RabbitMqOptions>().BindConfiguration(sectionName);
    }
}
