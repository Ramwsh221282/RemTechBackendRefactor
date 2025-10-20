using Microsoft.Extensions.DependencyInjection;

namespace RemTech.Shared.Configuration.Options;

public sealed class SeqOptions : IConfigurationPart
{
    public string Host { get; set; } = null!;

    public void Configure(IServiceCollection services) =>
        services.AddOptions<SeqOptions>().BindConfiguration(nameof(SeqOptions));
}
