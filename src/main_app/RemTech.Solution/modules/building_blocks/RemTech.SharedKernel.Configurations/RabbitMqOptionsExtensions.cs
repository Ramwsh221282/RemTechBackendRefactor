using Microsoft.Extensions.DependencyInjection;

namespace RemTech.SharedKernel.Configurations;

/// <summary>
/// Расширения для регистрации RabbitMqOptions в контейнере служб.
/// </summary>
public static class RabbitMqOptionsExtensions
{
	extension(IServiceCollection services)
	{
		public void AddRabbitMqOptionsFromAppsettings(string sectionName = nameof(RabbitMqOptions))
		{
			services.AddOptions<RabbitMqOptions>().BindConfiguration(sectionName);
		}
	}
}
