using Identity.Infrastructure.Common;
using Identity.WebApi.Options;
using RemTech.SharedKernel.Configurations;

namespace WebHostApplication.Injection;

/// <summary>
/// Регистрация конфигураций из appsettings.json.
/// </summary>
public static class AppsettingsConfigurationInjection
{
	extension(IServiceCollection services)
	{
		public void RegisterConfigurationFromAppsettings()
		{
			services.AddOptions<AesEncryptionOptions>().BindConfiguration(nameof(AesEncryptionOptions));
			services.AddOptions<EmbeddingsProviderOptions>().BindConfiguration(nameof(EmbeddingsProviderOptions));
			services.AddOptions<FrontendOptions>().BindConfiguration(nameof(FrontendOptions));
			services.AddOptions<NpgSqlOptions>().BindConfiguration(nameof(NpgSqlOptions));
			services.AddOptions<RabbitMqOptions>().BindConfiguration(nameof(RabbitMqOptions));
			services.AddOptions<SuperUserCredentialsOptions>().BindConfiguration(nameof(SuperUserCredentialsOptions));
			services.AddOptions<BcryptWorkFactorOptions>().BindConfiguration(nameof(BcryptWorkFactorOptions));
			services.AddOptions<JwtOptions>().BindConfiguration(nameof(JwtOptions));
			services.AddOptions<CachingOptions>().BindConfiguration(nameof(CachingOptions));
		}
	}
}
