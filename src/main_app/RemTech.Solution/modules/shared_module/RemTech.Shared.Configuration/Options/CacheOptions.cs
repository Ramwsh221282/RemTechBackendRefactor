using Microsoft.Extensions.DependencyInjection;

namespace RemTech.Shared.Configuration.Options;

public sealed class CacheOptions : IConfigurationPart
{
    public string Host { get; set; } = null!;

    public void Configure(IServiceCollection services) =>
        services.AddOptions<CacheOptions>().BindConfiguration(nameof(CacheOptions));
}
