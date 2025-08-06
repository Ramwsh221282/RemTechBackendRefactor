using Npgsql;
using RabbitMQ.Client;
using RemTech.Bootstrap.Api.Configuration;
using Serilog;
using StackExchange.Redis;

namespace RemTech.Bootstrap.Api.Injection;

public static class CommonInfrastructureInjection
{
    public static void InjectCommonInfrastructure(
        this IServiceCollection services,
        RemTechApplicationSettings settings
    )
    {
        services.InjectDatabase(settings.Database);
        services.InjectCache(settings.Cache);
        services.InjectRabbitMq(settings.RabbitMq);
        services.InjectLogging();
    }

    private static void InjectDatabase(
        this IServiceCollection services,
        RemTechDatabaseSettings settings
    )
    {
        services.AddSingleton(new NpgsqlDataSourceBuilder(settings.ToConnectionString()).Build());
    }

    private static void InjectCache(this IServiceCollection services, RemTechCacheSettings settings)
    {
        services.AddSingleton(ConnectionMultiplexer.Connect(settings.Host));
    }

    private static void InjectLogging(this IServiceCollection services)
    {
        services.AddSingleton<Serilog.ILogger>(
            new LoggerConfiguration().WriteTo.Console().CreateLogger()
        );
    }

    private static void InjectRabbitMq(
        this IServiceCollection services,
        RemTechRabbitMqSettings settings
    )
    {
        services.AddSingleton(
            new ConnectionFactory()
            {
                HostName = settings.HostName,
                UserName = settings.UserName,
                Password = settings.Password,
                Port = int.Parse(settings.Port),
            }
        );
    }
}
