using Serilog;
using Shared.Infrastructure.Module.Postgres;
using Shared.Infrastructure.Module.Postgres.Embeddings;
using Shared.Infrastructure.Module.RabbitMq;
using Shared.Infrastructure.Module.Redis;

namespace RemTech.Bootstrap.Api.Injection;

public static class CommonInfrastructureInjection
{
    public static void InjectCommonInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IEmbeddingGenerator, OnnxEmbeddingGenerator>();
        services.AddSingleton<RedisCache>();
        services.AddSingleton<PostgresDatabase>();
        services.AddSingleton<RabbitMqConnection>();
        services.AddSingleton<Serilog.ILogger>(
            new LoggerConfiguration().WriteTo.Console().CreateLogger()
        );
    }
}
