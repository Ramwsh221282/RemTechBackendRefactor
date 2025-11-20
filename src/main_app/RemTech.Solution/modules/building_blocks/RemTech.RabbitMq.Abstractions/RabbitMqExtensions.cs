using Microsoft.Extensions.DependencyInjection;
using RemTech.RabbitMq.Abstractions.Publishers;

namespace RemTech.RabbitMq.Abstractions;

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