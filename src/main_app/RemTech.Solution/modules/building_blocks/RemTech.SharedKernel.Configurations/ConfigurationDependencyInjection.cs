using DotNetEnv.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RemTech.SharedKernel.Configurations;

public static class ConfigurationDependencyInjection
{
	private const string DEFAULT_APPSETTINGS_FILE_PATH = "appsettings.json";
	private const string DEFAULT_ENV_FILE_PATH = ".env";

	public static void Register(this IConfigurationManager manager, IServiceCollection services, bool isDevelopment)
	{
		ReadConfigurationSource(manager, isDevelopment);
		BindConfigurationOptions(services);
	}

	private static void ReadConfigurationSource(IConfigurationManager manager, bool isDevelopment)
	{
		if (isDevelopment)
		{
			RegisterAppsettingsFile(manager);
		}
		else
		{
			ReadEnvironmentVariablesFile(manager);
		}
	}

	private static void RegisterAppsettingsFile(IConfigurationManager manager)
	{
		if (!File.Exists(DEFAULT_APPSETTINGS_FILE_PATH))
		{
			throw new FileNotFoundException($"The {DEFAULT_APPSETTINGS_FILE_PATH} file was not found.");
		}

		manager.AddJsonFile(DEFAULT_APPSETTINGS_FILE_PATH, optional: false, reloadOnChange: true);
	}

	private static void ReadEnvironmentVariablesFile(IConfigurationManager manager)
	{
		if (!File.Exists(DEFAULT_ENV_FILE_PATH))
		{
			throw new FileNotFoundException($"The {DEFAULT_ENV_FILE_PATH} file was not found.");
		}

		manager.AddDotNetEnv(path: DEFAULT_ENV_FILE_PATH);
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
