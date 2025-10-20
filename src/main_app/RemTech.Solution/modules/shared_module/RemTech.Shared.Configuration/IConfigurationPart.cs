using Microsoft.Extensions.DependencyInjection;

namespace RemTech.Shared.Configuration;

public interface IConfigurationPart
{
    void Configure(IServiceCollection services);
}
