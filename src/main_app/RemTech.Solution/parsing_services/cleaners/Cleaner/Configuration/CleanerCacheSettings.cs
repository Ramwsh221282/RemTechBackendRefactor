using StackExchange.Redis;

namespace Cleaner.Configuration;

internal sealed class CleanerCacheSettings
{
    private readonly string _host;

    private CleanerCacheSettings(string host) => _host = host;

    public static CleanerCacheSettings FromJson(string json)
    {
        IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile(json).Build();
        string? host = root.GetSection(ConfigurationConstants.REDIS_HOST_KEY).Value;
        return string.IsNullOrWhiteSpace(host)
            ? throw new ApplicationException(
                $"{ConfigurationConstants.REDIS_HOST_KEY} is not provided."
            )
            : new CleanerCacheSettings(host);
    }

    public void Register(IServiceCollection services)
    {
        services.AddSingleton<ConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(_host));
    }
}
