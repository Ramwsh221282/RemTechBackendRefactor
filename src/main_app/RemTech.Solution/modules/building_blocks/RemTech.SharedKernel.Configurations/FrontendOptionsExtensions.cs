using Microsoft.Extensions.DependencyInjection;

namespace RemTech.SharedKernel.Configurations;

public static class FrontendOptionsExtensions
{
	extension(IServiceCollection services)
	{
		public void AddFromAppsettings(string sectionName = nameof(FrontendOptions)) =>
			services.AddOptions<FrontendOptions>().BindConfiguration(sectionName);
	}
}
