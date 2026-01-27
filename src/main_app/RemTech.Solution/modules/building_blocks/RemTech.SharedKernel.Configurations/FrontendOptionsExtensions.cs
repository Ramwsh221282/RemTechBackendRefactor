using Microsoft.Extensions.DependencyInjection;

namespace RemTech.SharedKernel.Configurations;

/// <summary>
/// Расширения для регистрации FrontendOptions в контейнере служб.
/// </summary>
public static class FrontendOptionsExtensions
{
	extension(IServiceCollection services)
	{
		public void AddFromAppsettings(string sectionName = nameof(FrontendOptions)) =>
			services.AddOptions<FrontendOptions>().BindConfiguration(sectionName);
	}
}
