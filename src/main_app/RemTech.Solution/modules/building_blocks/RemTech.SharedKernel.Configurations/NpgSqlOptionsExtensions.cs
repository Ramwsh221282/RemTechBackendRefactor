using Microsoft.Extensions.DependencyInjection;

namespace RemTech.SharedKernel.Configurations;

/// <summary>
/// Расширения для регистрации NpgSqlOptions в контейнере служб.
/// </summary>
public static class NpgSqlOptionsExtensions
{
	extension(IServiceCollection services)
	{
		public void AddNpgSqlOptionsFromAppsettings(string sectionName = nameof(NpgSqlOptions))
		{
			services.AddOptions<NpgSqlOptions>().BindConfiguration(sectionName);
		}
	}
}
