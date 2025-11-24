using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Infrastructure.RabbitMq.Publishers;

namespace RemTech.SharedKernel.Infrastructure.RabbitMq;

public static class RabbitMqExtensions
{
    extension(IServiceCollection services)
    {
        public void AddRabbitMq()
        {
            services.AddOptions<RabbitMqConnectionOptions>().BindConfiguration(nameof(RabbitMqConnectionOptions));
            services.AddSingleton<RabbitMqConnectionSource>();
            services.AddSingleton<RabbitMqPublishers>();
        }
    }
}