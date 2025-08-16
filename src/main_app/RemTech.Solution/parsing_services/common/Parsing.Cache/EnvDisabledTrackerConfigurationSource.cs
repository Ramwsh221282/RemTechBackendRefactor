namespace Parsing.Cache;

public sealed class EnvDisabledTrackerConfigurationSource : IDisabledTrackerConfigurationSource
{
    public DisabledTrackerConfiguration Provide()
    {
        string? hostName = Environment.GetEnvironmentVariable("REDIS_HOST");
        if (string.IsNullOrWhiteSpace(hostName))
            throw new ApplicationException("Rabbit mq configuration. REDIS_HOST not found.");
        return new DisabledTrackerConfiguration(hostName);
    }
}
