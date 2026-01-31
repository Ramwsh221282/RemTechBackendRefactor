using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace RemTech.SharedKernel.Web;

/// <summary>
/// Настройки фронтенда.
/// </summary>
public sealed class FrontendOptions
{
	/// <summary>
	/// URL фронтенда.
	/// </summary>
	public string FrontendUrl { get; set; } = string.Empty;

	/// <summary>
	/// Регистрация FrontendOptions в контейнере служб.
	/// </summary>
	/// <param name="services">Контейнер служб для регистрации настроек.</param>
	public static void AddFrontendOptions(IServiceCollection services)
	{
		services.AddOptions<FrontendOptions>().BindConfiguration(nameof(FrontendOptions));
	}

	/// <summary>
	/// Перерегистрация FrontendOptions с новыми значениями.
	/// </summary>
	/// <param name="services">Контейнер служб для перерегистрации настроек.</param>
	/// <param name="reconfigured">Новые значения настроек фронтенда.</param>
	public static void ReconfigureOptions(IServiceCollection services, FrontendOptions reconfigured)
	{
		services.RemoveAll<IOptions<FrontendOptions>>();
		IOptions<FrontendOptions> options = Options.Create(reconfigured);
		services.AddSingleton(options);
	}
}
