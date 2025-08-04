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
        IConfigurationSection section = root.GetSection(nameof(RemTechApplicationSettings))
            .GetSection(nameof(RemTechCacheSettings));
        string? host = section.GetValue<string>("Host");
        if (string.IsNullOrWhiteSpace(host))
            throw new ApplicationException($"{nameof(RemTechCacheSettings)} host string is empty.");
        return new RemTechCacheSettings(host);
    }
}
