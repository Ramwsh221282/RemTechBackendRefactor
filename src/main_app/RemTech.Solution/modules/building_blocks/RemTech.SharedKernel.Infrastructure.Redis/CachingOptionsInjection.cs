using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Configurations;

namespace RemTech.SharedKernel.Infrastructure.Redis;

/// <summary>
/// Расширения для регистрации CachingOptions в контейнере служб.
/// </summary>
public static class CachingOptionsInjection
{
	extension(IServiceCollection services)
	{
		public void AddCachingOptionsFromAppsettings(string sectionName = nameof(CachingOptions))
		{
			services.AddOptions<CachingOptions>().BindConfiguration(sectionName);
		}
	}
}
