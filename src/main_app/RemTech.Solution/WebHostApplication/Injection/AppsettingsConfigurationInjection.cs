using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Configurations;

namespace WebHostApplication.Injection;

/// <summary>
/// Регистрация конфигураций из appsettings.json.
/// </summary>
public static class AppsettingsConfigurationInjection
{
	extension(WebApplication app)
	{
		public async Task ValidateConfigurations()
		{
			await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
			IServiceProvider sp = scope.ServiceProvider;
			EmbeddingsProviderOptions embeddings = GetConfiguration<EmbeddingsProviderOptions>(sp);

			List<string> errors = [];

			FrontendOptions frontend = GetConfiguration<FrontendOptions>(sp);
			NpgSqlOptions npgSql = GetConfiguration<NpgSqlOptions>(sp);
			RabbitMqOptions rabbitMq = GetConfiguration<RabbitMqOptions>(sp);
			SuperUserCredentialsOptions superUser = GetConfiguration<SuperUserCredentialsOptions>(sp);
			BcryptWorkFactorOptions bcrypt = GetConfiguration<BcryptWorkFactorOptions>(sp);
			JwtOptions jwt = GetConfiguration<JwtOptions>(sp);
			CachingOptions caching = GetConfiguration<CachingOptions>(sp);
			AesEncryptionOptions aes = GetConfiguration<AesEncryptionOptions>(sp);

			ValidateEmbeddingsProviderOptions(errors, embeddings);
			ValidateFrontendOptions(errors, frontend);
			ValidateJwtOptions(errors, jwt);
			ValidateCacheOptions(errors, caching);
			ValidateAesOptions(errors, aes);
			ValidateSuperUserOptions(errors, superUser);
			ValidateBcryptOptions(errors, bcrypt);
			ValidateRabbitMqOptions(errors, rabbitMq);
			ValidateNpgSqlOptions(errors, npgSql);
		}
	}

	private static void ThrowIfContainsErrors(List<string> errors)
	{
		if (errors is { Count: 0 })
		{
			return;
		}

		string messages = string.Join(Environment.NewLine, errors);
		throw new ApplicationException($"Ошибки в конфигурации приложения:{Environment.NewLine}{messages}");
	}

	private static void ValidateEmbeddingsProviderOptions(List<string> errors, EmbeddingsProviderOptions options)
	{
		if (string.IsNullOrWhiteSpace(options.ModelPath))
		{
			errors.Add("EmbeddingsProviderOptions.ModelPath не задан.");
		}
	}

	private static void ValidateFrontendOptions(List<string> errors, FrontendOptions options)
	{
		if (string.IsNullOrWhiteSpace(options.Url))
		{
			errors.Add("FrontendOptions.Url не задан.");
		}
	}

	private static void ValidateNpgSqlOptions(List<string> errors, NpgSqlOptions options)
	{
		if (string.IsNullOrWhiteSpace(options.Host))
		{
			errors.Add("NpgSqlOptions.Host не задан.");
		}

		if (string.IsNullOrWhiteSpace(options.Port))
		{
			errors.Add("NpgSqlOptions.Port не задан.");
		}

		if (string.IsNullOrWhiteSpace(options.Database))
		{
			errors.Add("NpgSqlOptions.Database не задан.");
		}

		if (string.IsNullOrWhiteSpace(options.Username))
		{
			errors.Add("NpgSqlOptions.Username не задан.");
		}

		if (string.IsNullOrWhiteSpace(options.Password))
		{
			errors.Add("NpgSqlOptions.Password не задан.");
		}
	}

	private static void ValidateRabbitMqOptions(List<string> errors, RabbitMqOptions options)
	{
		if (string.IsNullOrWhiteSpace(options.Hostname))
		{
			errors.Add("RabbitMqOptions.Hostname не задан.");
		}

		if (string.IsNullOrWhiteSpace(options.Username))
		{
			errors.Add("RabbitMqOptions.Username не задан.");
		}

		if (string.IsNullOrWhiteSpace(options.Password))
		{
			errors.Add("RabbitMqOptions.Password не задан.");
		}

		if (options.Port == 0)
		{
			errors.Add("RabbitMqOptions.Port не задан.");
		}
	}

	private static void ValidateBcryptOptions(List<string> errors, BcryptWorkFactorOptions options)
	{
		if (options.WorkFactor == 0)
		{
			errors.Add("BcryptWorkFactorOptions.WorkFactor не задан.");
		}
	}

	private static void ValidateSuperUserOptions(List<string> errors, SuperUserCredentialsOptions options)
	{
		if (string.IsNullOrWhiteSpace(options.Login))
		{
			errors.Add("SuperUserCredentialsOptions.Login не задан.");
		}

		if (string.IsNullOrWhiteSpace(options.Password))
		{
			errors.Add("SuperUserCredentialsOptions.Password не задан.");
		}

		if (string.IsNullOrWhiteSpace(options.Email))
		{
			errors.Add("SuperUserCredentialsOptions.Email не задан.");
		}
	}

	private static void ValidateAesOptions(List<string> errors, AesEncryptionOptions options)
	{
		if (string.IsNullOrWhiteSpace(options.PlainKey))
		{
			errors.Add("AesEncryptionOptions.PlainKey не задан.");
		}
	}

	private static void ValidateJwtOptions(List<string> errors, JwtOptions options)
	{
		if (string.IsNullOrWhiteSpace(options.Issuer))
		{
			errors.Add("JwtOptions.Issuer не задан.");
		}

		if (string.IsNullOrWhiteSpace(options.SecretKey))
		{
			errors.Add("JwtOptions.SecretKey не задан.");
		}

		if (string.IsNullOrWhiteSpace(options.Audience))
		{
			errors.Add("JwtOptions.Audience не задан.");
		}
	}

	private static void ValidateCacheOptions(List<string> errors, CachingOptions caching)
	{
		if (string.IsNullOrWhiteSpace(caching.RedisConnectionString))
		{
			errors.Add("CachingOptions.RedisConnectionString не задан.");
		}

		if (caching.LocalCacheExpirationMinutes == 0)
		{
			errors.Add("CachingOptions.LocalCacheExpirationMinutes не задан.");
		}

		if (caching.CacheExpirationMinutes == 0)
		{
			errors.Add("CachingOptions.CacheExpirationMinutes не задан.");
		}
	}

	private static T GetConfiguration<T>(IServiceProvider services)
		where T : class
	{
		return services.GetRequiredService<IOptions<T>>().Value;
	}
}
