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
