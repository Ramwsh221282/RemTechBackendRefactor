namespace Parsing.Cache;

public sealed class DisabledTrackerConfigurationSource : IDisabledTrackerConfigurationSource
{
    public DisabledTrackerConfiguration Provide()
    {
        try
        {
            return new JsonDisabledTrackerConfigurationSource("appsettings.json").Provide();
        }
        catch
        {
            return new EnvDisabledTrackerConfigurationSource().Provide();
        }
    }
}
