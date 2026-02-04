using Identity.Infrastructure.Common;
using Identity.WebApi.Options;
using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Configurations;
using Sprache;
using static System.Net.Mime.MediaTypeNames;

namespace WebHostApplication.Injection;

/// <summary>
/// Регистрация конфигураций из appsettings.json.
/// </summary>
public static class AppsettingsConfigurationInjection
{
	extension(WebApplicationBuilder builder)
	{
		public void RegisterConfiguration()
		{
			bool isDevelopment = builder.Environment.IsDevelopment();

			Action sourceRegistration = isDevelopment
				? builder.RegisterEnvironmentVariablesConfigurationSource
				: builder.RegisterAppsettingsConfigurationSource;

			builder.Services.RegisterConfiguration();
		}

		private void RegisterAppsettingsConfigurationSource()
		{
			builder.Configuration.AddJsonFile("appsettings.json");
		}

		private void RegisterEnvironmentVariablesConfigurationSource()
		{
			DotNetEnv.Env.Load();
			builder.Configuration.AddEnvironmentVariables();
		}
	}

	extension(IServiceCollection services)
	{
		private void RegisterConfiguration()
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

	extension(WebApplication app)
	{
		public async Task ValidateConfigurations()
		{
			await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
			IServiceProvider sp = scope.ServiceProvider;

			AesEncryptionOptions aesOptions = GetConfiguration<AesEncryptionOptions>(sp);
			if (string.IsNullOrWhiteSpace(aesOptions.PlainKey))
			{
				throw new ApplicationException("AesEncryptionOptions.PlainKey is empty.");
			}

			EmbeddingsProviderOptions embeddings = GetConfiguration<EmbeddingsProviderOptions>(sp);
			if (string.IsNullOrWhiteSpace(embeddings.ModelPath))
			{
				throw new ApplicationException("EmbeddingsProviderOptions.ModelPath is empty.");
			}
			if (!File.Exists(embeddings.ModelPath))
			{
				throw new ApplicationException(
					$"EmbeddingsProviderOptions.ModelPath file not found: {embeddings.ModelPath}"
				);
			}
			if (string.IsNullOrWhiteSpace(embeddings.TokenizerPath))
			{
				throw new ApplicationException("EmbeddingsProviderOptions.TokenizerPath is empty.");
			}

			FrontendOptions frontend = GetConfiguration<FrontendOptions>(sp);
			if (string.IsNullOrWhiteSpace(frontend.Url))
			{
				throw new ApplicationException("FrontendOptions.Url is empty.");
			}

			NpgSqlOptions npgSql = GetConfiguration<NpgSqlOptions>(sp);
			if (string.IsNullOrWhiteSpace(npgSql.Database))
			{
				throw new ApplicationException("NpgSqlOptions.Database is empty.");
			}
			if (string.IsNullOrWhiteSpace(npgSql.Username))
			{
				throw new ApplicationException("NpgSqlOptions.Username is empty.");
			}
			if (string.IsNullOrWhiteSpace(npgSql.Password))
			{
				throw new ApplicationException("NpgSqlOptions.Password is empty.");
			}
			if (string.IsNullOrWhiteSpace(npgSql.Host))
			{
				throw new ApplicationException("NpgSqlOptions.Host is empty.");
			}
			if (string.IsNullOrWhiteSpace(npgSql.Port))
			{
				throw new ApplicationException("NpgSqlOptions.Port is empty.");
			}

			RabbitMqOptions rabbitMq = GetConfiguration<RabbitMqOptions>(sp);
			if (string.IsNullOrWhiteSpace(rabbitMq.Hostname))
			{
				throw new ApplicationException("RabbitMqOptions.HostName is empty.");
			}
			if (string.IsNullOrWhiteSpace(rabbitMq.Username))
			{
				throw new ApplicationException("RabbitMqOptions.Username is empty.");
			}
			if (string.IsNullOrWhiteSpace(rabbitMq.Password))
			{
				throw new ApplicationException("RabbitMqOptions.Password is empty.");
			}
			if (rabbitMq.Port == default)
			{
				throw new ApplicationException("RabbitMqOptions.Port is empty.");
			}

			SuperUserCredentialsOptions superUser = GetConfiguration<SuperUserCredentialsOptions>(sp);
			if (string.IsNullOrWhiteSpace(superUser.Email))
			{
				throw new ApplicationException("SuperUserCredentialsOptions.Email is empty.");
			}
			if (string.IsNullOrWhiteSpace(superUser.Password))
			{
				throw new ApplicationException("SuperUserCredentialsOptions.Password is empty.");
			}
			if (string.IsNullOrWhiteSpace(superUser.Login))
			{
				throw new ApplicationException("SuperUserCredentialsOptions.Login is empty.");
			}

			BcryptWorkFactorOptions bcrypt = GetConfiguration<BcryptWorkFactorOptions>(sp);
			if (bcrypt.WorkFactor == default)
			{
				throw new ApplicationException("BcryptWorkFactorOptions.WorkFactor is empty.");
			}

			JwtOptions jwt = GetConfiguration<JwtOptions>(sp);
			if (string.IsNullOrWhiteSpace(jwt.Issuer))
			{
				throw new ApplicationException("JwtOptions.Issuer is empty.");
			}
			if (string.IsNullOrWhiteSpace(jwt.SecretKey))
			{
				throw new ApplicationException("JwtOptions.SecretKey is empty.");
			}
			if (string.IsNullOrWhiteSpace(jwt.Audience))
			{
				throw new ApplicationException("JwtOptions.Audience is empty.");
			}
		}
	}

	private static void ValidateJwtOptions(JwtOptions options)
	{
		if (string.IsNullOrWhiteSpace(options.Issuer))
		{
			throw new ApplicationException("JwtOptions.Issuer is empty.");
		}
		if (string.IsNullOrWhiteSpace(options.SecretKey))
		{
			throw new ApplicationException("JwtOptions.SecretKey is empty.");
		}
		if (string.IsNullOrWhiteSpace(options.Audience))
		{
			throw new ApplicationException("JwtOptions.Audience is empty.");
		}
	}

	private static void ValidateCacheOptions(CachingOptions caching)
	{
		if (string.IsNullOrWhiteSpace(caching.RedisConnectionString))
		{
			throw new ApplicationException("CachingOptions.RedisConnectionString is empty.");
		}
		if (caching.LocalCacheExpirationMinutes == default)
		{
			throw new ApplicationException("CachingOptions.LocalCacheExpirationMinutes is empty.");
		}
		if (caching.CacheExpirationMinutes == default)
		{
			throw new ApplicationException("CachingOptions.CacheExpirationMinutes is empty.");
		}
	}

	private static T GetConfiguration<T>(IServiceProvider services)
		where T : class
	{
		return services.GetRequiredService<IOptions<T>>().Value;
	}
}
