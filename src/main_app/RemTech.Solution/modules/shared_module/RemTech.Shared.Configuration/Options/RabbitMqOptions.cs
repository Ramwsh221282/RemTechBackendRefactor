using Microsoft.Extensions.DependencyInjection;

namespace RemTech.Shared.Configuration.Options;

public sealed record RabbitMqOptions : IConfigurationPart
{
    public string HostName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Port { get; set; } = null!;

    public void Configure(IServiceCollection services) =>
        services.AddOptions<RabbitMqOptions>().BindConfiguration(nameof(RabbitMqOptions));
}