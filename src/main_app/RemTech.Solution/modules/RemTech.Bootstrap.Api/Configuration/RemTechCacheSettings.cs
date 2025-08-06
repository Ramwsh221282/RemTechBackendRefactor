namespace RemTech.Bootstrap.Api.Configuration;

public sealed record RemTechCacheSettings
{
    public string Host { get; }

    private RemTechCacheSettings(string host)
    {
        Host = host;
    }

    public static RemTechCacheSettings CreateFromJson(string jsonPath)
    {
        IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile(jsonPath).Build();
        string? host = root.GetSection(ConfigurationConstants.REDIS_HOST_KEY).Value;
        return string.IsNullOrWhiteSpace(host)
            ? throw new ApplicationException(
                $"{ConfigurationConstants.REDIS_HOST_KEY} is not provided."
            )
            : new RemTechCacheSettings(host);
    }
}
