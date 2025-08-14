namespace RemTech.Bootstrap.Api.Configuration;

public sealed class RemTechSeqSettings
{
    public string Host { get; }

    private RemTechSeqSettings(string host)
    {
        Host = host;
    }

    public static RemTechSeqSettings CreateFromJson(string json)
    {
        IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile(json).Build();
        string? host = root.GetSection(ConfigurationConstants.SEQ_HOST_KEY).Value;
        return string.IsNullOrWhiteSpace(host)
            ? throw new ApplicationException(
                $"{ConfigurationConstants.SEQ_HOST_KEY} is not provided."
            )
            : new RemTechSeqSettings(host);
    }
}
