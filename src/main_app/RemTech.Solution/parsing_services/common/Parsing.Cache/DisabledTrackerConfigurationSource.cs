namespace Parsing.Cache;

public sealed class DisabledTrackerConfigurationSource(bool isDevelopment)
    : IDisabledTrackerConfigurationSource
{
    public DisabledTrackerConfiguration Provide()
    {
        if (isDevelopment)
        {
            Console.WriteLine("Development environment.");
            return new JsonDisabledTrackerConfigurationSource("appsettings.json").Provide();
        }
        Console.WriteLine("Production environment.");
        return new EnvDisabledTrackerConfigurationSource().Provide();
    }
}
