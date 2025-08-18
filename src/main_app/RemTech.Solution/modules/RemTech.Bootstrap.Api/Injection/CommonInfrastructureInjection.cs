using Npgsql;
using RabbitMQ.Client;
using RemTech.Bootstrap.Api.Configuration;
using Serilog;
using Shared.Infrastructure.Module.Postgres.Embeddings;
using StackExchange.Redis;

namespace RemTech.Bootstrap.Api.Injection;

public static class CommonInfrastructureInjection
{
    public static void InjectCommonInfrastructure(
        this IServiceCollection services,
        RemTechApplicationSettings settings
    )
    {
        services.AddSingleton<IEmbeddingGenerator, OnnxEmbeddingGenerator>();
        services.InjectDatabase(settings.Database);
        services.InjectRabbitMq(settings.RabbitMq);
        services.InjectCache(settings.Cache);
        services.InjectLogging(settings.Seq);
    }

    private static void InjectCache(this IServiceCollection services, RemTechCacheSettings settings)
    {
        ConfigurationOptions opts = new ConfigurationOptions()
        {
            AbortOnConnectFail = false,
            AsyncTimeout = 5000,
            AllowAdmin = true,
            EndPoints = { settings.Host },
            ConnectTimeout = 5000,
            ConnectRetry = 3,
        };
        ConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(opts);
        IDatabase database = multiplexer.GetDatabase();
        Guid id = Guid.NewGuid();
        string key = id.ToString();
        database.StringSet(key, "test");
        database.KeyDelete(key);
        services.AddSingleton(multiplexer);
    }

    private static void InjectDatabase(
        this IServiceCollection services,
        RemTechDatabaseSettings settings
    )
    {
        services.AddSingleton(_ =>
        {
            NpgsqlDataSourceBuilder builder = new NpgsqlDataSourceBuilder(
                settings.ToConnectionString()
            );
            builder.UseVector();
            return builder.Build();
        });
    }

    private static void InjectLogging(this IServiceCollection services, RemTechSeqSettings settings)
    {
        services.AddSingleton<Serilog.ILogger>(
            new LoggerConfiguration().WriteTo.Seq(settings.Host).WriteTo.Console().CreateLogger()
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
