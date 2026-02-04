using DotNetEnv.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RemTech.SharedKernel.Configurations;

public static class ConfigurationDependencyInjection
{
	public static void Register(this IConfigurationManager manager, IServiceCollection services, bool isDevelopment)
	{
		ReadConfigurationSource(manager, isDevelopment);
		BindConfigurationOptions(services);
	}

	private static void ReadConfigurationSource(IConfigurationManager manager, bool isDevelopment)
	{
		if (isDevelopment)
		{
			ReadEnvironmentVariablesFile(manager);
		}
		else
		{
			RegisterAppsettingsFile(manager);
		}
	}

	private static void RegisterAppsettingsFile(IConfigurationManager manager)
	{
		if (!File.Exists("appsettings.json"))
		{
			throw new FileNotFoundException("The appsettings.json file was not found.");
		}

		manager.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
	}

	private static void ReadEnvironmentVariablesFile(IConfigurationManager manager)
	{
		if (!File.Exists(".env"))
		{
			throw new FileNotFoundException("The .env file was not found.");
		}

		manager.AddDotNetEnv();
	}

	private static void BindConfigurationOptions(IServiceCollection services)
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
