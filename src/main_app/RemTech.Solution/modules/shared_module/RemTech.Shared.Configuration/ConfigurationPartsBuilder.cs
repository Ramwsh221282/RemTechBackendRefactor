using Microsoft.Extensions.DependencyInjection;

namespace RemTech.Shared.Configuration;

public sealed class ConfigurationPartsBuilder
{
    private readonly ConfigurationInjector _injector;
    private readonly ConfigurationPrinter _printer;

    public ConfigurationPartsBuilder(IServiceCollection services)
    {
        _injector = new ConfigurationInjector(services);
        _printer = new ConfigurationPrinter();
    }

    public void AddConfigurations()
    {
        IEnumerable<IConfigurationPart> injected = _injector.Inject();
        _printer.PrintConfigurations(injected);
    }
}
