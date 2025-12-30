using Microsoft.Extensions.DependencyInjection;

namespace RemTech.SharedKernel.Configurations;

public static class EmbeddingsProviderExtensions
{
    extension(IServiceCollection services)
    {
        public void RegisterFromAppsettings()
        {
            services.AddOptions<EmbeddingsProviderOptions>().BindConfiguration(nameof(EmbeddingsProviderOptions));
        }
    }
}