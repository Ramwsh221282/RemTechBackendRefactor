using Microsoft.Extensions.Configuration;

namespace Parsing.Cache;

public sealed class JsonDisabledTrackerConfigurationSource(string filePath)
    : IDisabledTrackerConfigurationSource
{
    public DisabledTrackerConfiguration Provide()
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ApplicationException(
                $"{nameof(JsonDisabledTrackerConfigurationSource)} file path is empty."
            );
        if (!File.Exists(filePath))
            throw new ApplicationException(
                $"{nameof(JsonDisabledTrackerConfigurationSource)} file does not exist."
            );
        IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile(filePath).Build();
        string? hostName = root.GetSection("REDIS_HOST").Value;
        if (string.IsNullOrWhiteSpace(hostName))
            throw new ApplicationException("Rabbit mq configuration. REDIS_HOST not found.");
        return new DisabledTrackerConfiguration(hostName);
    }
}
