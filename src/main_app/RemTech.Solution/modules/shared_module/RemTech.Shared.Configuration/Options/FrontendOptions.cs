using Microsoft.Extensions.DependencyInjection;

namespace RemTech.Shared.Configuration.Options;

public sealed class FrontendOptions : IConfigurationPart
{
    public string FrontendUrl { get; set; } = null!;

    public void Configure(IServiceCollection services) =>
        services.AddOptions<FrontendOptions>().BindConfiguration(nameof(FrontendOptions));
}
