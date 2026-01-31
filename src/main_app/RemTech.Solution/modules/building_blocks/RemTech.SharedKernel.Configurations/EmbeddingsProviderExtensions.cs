using Microsoft.Extensions.DependencyInjection;

namespace RemTech.SharedKernel.Configurations;

/// <summary>
/// Расширения для регистрации EmbeddingsProviderOptions в контейнере служб.
/// </summary>
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
