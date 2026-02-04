using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Configurations;

namespace Identity.Infrastructure.Common;

/// <summary>
/// Расширения для регистрации JwtOptions в контейнере служб.
/// </summary>
public static class JwtSecretKeyInjection
{
	extension(IServiceCollection services)
	{
		public void AddJwtOptionsFromAppsettings(string sectionName = nameof(JwtOptions))
		{
			services.AddOptions<JwtOptions>().BindConfiguration(sectionName);
		}
	}
}
