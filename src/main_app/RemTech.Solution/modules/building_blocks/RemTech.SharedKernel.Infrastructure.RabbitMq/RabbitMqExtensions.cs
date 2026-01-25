using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scrutor;

namespace RemTech.SharedKernel.Infrastructure.RabbitMq;

public static class RabbitMqExtensions
{
    extension(IServiceCollection services)
    {
        public void AddRabbitMq()
        {
            services.TryAddSingleton<RabbitMqConnectionSource>();
            services.TryAddSingleton<RabbitMqProducer>();
            services.AddHostedService<AggregatedConsumersHostedService>();
        }

        public void AddAggregatedConsumersBackgroundService() =>
            services.TryAddSingleton<AggregatedConsumersHostedService>();

        public void AddConsumersFromAssemblies(IEnumerable<Assembly> assemblies) =>
            services.Scan(x =>
                x.FromAssemblies(assemblies)
                    .AddClasses(classes => classes.AssignableTo<IConsumer>())
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithSingletonLifetime()
            );
    }
}
