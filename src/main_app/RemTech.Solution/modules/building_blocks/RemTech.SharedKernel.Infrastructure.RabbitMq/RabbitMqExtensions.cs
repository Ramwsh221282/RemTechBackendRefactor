using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Configurations;

namespace RemTech.SharedKernel.Infrastructure.RabbitMq;

public static class RabbitMqExtensions
{
    extension(IServiceCollection services)
    {
        public void AddRabbitMq()
        {
            if (!services.EnsureOptionsExists())
                throw new InvalidOperationException("RabbitMqOptions was not registered. Please register it using services.Configure<RabbitMqOptions>(...) or services.AddOptions<RabbitMqOptions>().BindConfiguration(...) before calling AddRabbitMq().");
            services.AddSingleton<RabbitMqConnectionSource>();
            services.AddSingleton<RabbitMqProducer>();
            services.AddHostedService<AggregatedConsumersHostedService>();
        }

        private bool EnsureOptionsExists()
        {
            return services.Any(s => s.ServiceType == typeof(IOptions<RabbitMqOptions>) ||
                                     s.ServiceType == typeof(IConfigureOptions<RabbitMqOptions>));
        }
    }
}