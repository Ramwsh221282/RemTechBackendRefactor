using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Common;

/// <summary>
/// Расширения для регистрации BcryptWorkFactorOptions в контейнере служб.
/// </summary>
public static class BcryptWorkFactorInjection
{
	extension(IServiceCollection services)
	{
		public void AddBcryptWorkFactorOptionsFromAppsettings(string sectionName = nameof(BcryptWorkFactorOptions))
		{
			services.AddOptions<BcryptWorkFactorOptions>().BindConfiguration(sectionName);
		}
	}
}
