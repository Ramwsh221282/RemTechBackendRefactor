using Microsoft.Extensions.DependencyInjection;

namespace RemTech.Shared.Configuration.Options;

public sealed class OnnxEmbeddingGeneratorOptions : IConfigurationPart
{
    public string TokenizerPath { get; set; } = null!;
    public string ModelPath { get; set; } = null!;

    public void Configure(IServiceCollection services) =>
        services
            .AddOptions<OnnxEmbeddingGeneratorOptions>()
            .BindConfiguration(nameof(OnnxEmbeddingGeneratorOptions));
}
