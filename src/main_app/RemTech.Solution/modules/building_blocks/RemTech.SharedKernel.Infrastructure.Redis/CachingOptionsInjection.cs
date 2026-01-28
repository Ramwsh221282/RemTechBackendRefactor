using Microsoft.Extensions.DependencyInjection;

namespace RemTech.SharedKernel.Infrastructure.Redis;

/// <summary>
/// Расширения для регистрации CachingOptions в контейнере служб.
/// </summary>
public static class CachingOptionsInjection
{
	extension(IServiceCollection services)
	{
		public void AddCachingOptionsFromAppsettings(string sectionName = nameof(CachingOptions)) =>
			services.AddOptions<CachingOptions>().BindConfiguration(sectionName);
	}
}
