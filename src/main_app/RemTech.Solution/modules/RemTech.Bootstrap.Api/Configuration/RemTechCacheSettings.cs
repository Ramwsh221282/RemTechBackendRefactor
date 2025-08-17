namespace RemTech.Bootstrap.Api.Configuration;

public sealed record RemTechCacheSettings
{
    private const string Key = ConfigurationConstants.REDIS_HOST_KEY;
    public string Host { get; }

    private RemTechCacheSettings(string host)
    {
        Console.WriteLine($"Cache host: {host}");
        Host = host;
    }

    public static RemTechCacheSettings CreateFromJson(string jsonPath)
    {
        IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile(jsonPath).Build();
        string? host = root.GetSection(Key).Value;
        return CreateFromValue(host);
    }

    public static RemTechCacheSettings FromEnv()
    {
        string? host = Environment.GetEnvironmentVariable(Key);
        return CreateFromValue(host);
    }

    private static RemTechCacheSettings CreateFromValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ApplicationException($"{Key} is not provided.")
            : new RemTechCacheSettings(value);
    }
}
