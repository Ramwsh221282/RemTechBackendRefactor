using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RemTech.Shared.Configuration;

internal sealed class ConfigurationInjector
{
    private readonly IServiceCollection _services;

    public ConfigurationInjector(IServiceCollection services) => _services = services;

    public IEnumerable<IConfigurationPart> Inject()
    {
        IEnumerable<Type> types = ScanTypes();
        IEnumerable<IConfigurationPart> parts = ConvertToInstances(types);
        InjectParts(parts);
        return parts;
    }

    private void InjectParts(IEnumerable<IConfigurationPart> parts)
    {
        foreach (IConfigurationPart part in parts)
            part.Configure(_services);
    }

    private IEnumerable<IConfigurationPart> ConvertToInstances(IEnumerable<Type> types)
    {
        List<IConfigurationPart> parts = [];
        foreach (var type in types)
        {
            IConfigurationPart? part = (IConfigurationPart?)Activator.CreateInstance(type);
            if (part == null)
                continue;
            parts.Add(part);
        }

        return parts;
    }

    private IEnumerable<Type> ScanTypes()
    {
        Type interfaceType = typeof(IConfiguration);
        IEnumerable<Type> types = interfaceType
            .Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && interfaceType.IsAssignableFrom(t));
        return types;
    }
}