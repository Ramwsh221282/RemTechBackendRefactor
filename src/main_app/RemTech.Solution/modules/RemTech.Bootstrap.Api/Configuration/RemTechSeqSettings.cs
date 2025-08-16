namespace RemTech.Bootstrap.Api.Configuration;

public sealed class RemTechSeqSettings
{
    private const string Key = ConfigurationConstants.SEQ_HOST_KEY;
    private const string Context = nameof(RemTechSeqSettings);
    public string Host { get; }

    private RemTechSeqSettings(string host)
    {
        Host = host;
    }

    public static RemTechSeqSettings CreateFromJson(string json)
    {
        IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile(json).Build();
        string? host = root.GetSection(Key).Value;
        return FromValue(host);
    }

    public static RemTechSeqSettings FromEnv()
    {
        string? host = Environment.GetEnvironmentVariable(Key);
        return FromValue(host);
    }

    private static RemTechSeqSettings FromValue(string? host)
    {
        return string.IsNullOrWhiteSpace(host)
            ? throw new ApplicationException($"{Context} is not provided.")
            : new RemTechSeqSettings(host);
    }
}
