using Parsing.SDK.Browsers;

namespace Cleaner.Configuration;

internal sealed class CleanerConfiguration
{
    private readonly CleanerRabbitMqSettings _rabbitMq;
    private readonly CleanerCacheSettings _cache;

    private CleanerConfiguration(CleanerRabbitMqSettings rabbitMq, CleanerCacheSettings cache)
    {
        _rabbitMq = rabbitMq;
        _cache = cache;
    }

    public static CleanerConfiguration FromJson(string json)
    {
        CleanerRabbitMqSettings rabbit = CleanerRabbitMqSettings.FromJson(json);
        CleanerCacheSettings cache = CleanerCacheSettings.FromJson(json);
        return new CleanerConfiguration(rabbit, cache);
    }

    public static CleanerConfiguration FromEnv()
    {
        CleanerRabbitMqSettings rabbit = CleanerRabbitMqSettings.FromEnv();
        CleanerCacheSettings cache = CleanerCacheSettings.FromEnv();
        return new CleanerConfiguration(rabbit, cache);
    }

    public static CleanerConfiguration ResolveByEnvironment(HostApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            Console.WriteLine("Development environment.");
            BrowserFactory.DevelopmentMode();
            return FromJson("appsettings.json");
        }
        Console.WriteLine("Production environment.");
        BrowserFactory.ProductionMode();
        return FromEnv();
    }

    public void Register(IServiceCollection services)
    {
        _rabbitMq.Register(services);
        _cache.Register(services);
    }
}
