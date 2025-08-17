using StackExchange.Redis;

namespace Cleaner.Configuration;

internal sealed class CleanerCacheSettings
{
    private const string Key = ConfigurationConstants.REDIS_HOST_KEY;
    private const string Context = nameof(CleanerCacheSettings);
    private readonly string _host;

    private CleanerCacheSettings(string host)
    {
        Console.WriteLine($"Cache host: {host}");
        _host = host;
    }

    public static CleanerCacheSettings FromJson(string json)
    {
        IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile(json).Build();
        string? host = root.GetSection(Key).Value;
        return FromValue(host);
    }

    public static CleanerCacheSettings FromEnv()
    {
        string? host = Environment.GetEnvironmentVariable(Key);
        return FromValue(host);
    }

    private static CleanerCacheSettings FromValue(string? host)
    {
        return string.IsNullOrWhiteSpace(host)
            ? throw new ApplicationException($"{Context} is not provided.")
            : new CleanerCacheSettings(host);
    }

    public void Register(IServiceCollection services)
    {
        services.AddSingleton<ConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(_host));
    }
}
