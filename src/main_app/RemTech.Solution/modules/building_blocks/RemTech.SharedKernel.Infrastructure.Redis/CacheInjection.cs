using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Configurations;

namespace RemTech.SharedKernel.Infrastructure.Redis;

/// <summary>
/// Расширения для регистрации гибридного кэша в контейнере служб.
/// </summary>
public static class CacheInjection
{
	extension(IServiceCollection services)
	{
		public void RegisterHybridCache(IConfigurationManager configuration)
		{
			RegisterCachingOptions(services, configuration);
		}
	}

	private static void RegisterCachingOptions(IServiceCollection services, IConfigurationManager configuration)
	{
		RemTech.SharedKernel.Configurations.CachingOptions cacheOpts = configuration
			.GetSection(nameof(CachingOptions))
			.Get<CachingOptions>();
		configuration.Bind(cacheOpts);

		if (cacheOpts is null)
		{
			throw new InvalidOperationException("CachingOptions не зарегистрирован в IConfigurationManager.");
		}

		cacheOpts.Validate();

		services.AddStackExchangeRedisCache(options =>
		{
			options.Configuration = cacheOpts.RedisConnectionString;
		});

		services.AddHybridCache(options =>
		{
			options.DefaultEntryOptions = new HybridCacheEntryOptions()
			{
				LocalCacheExpiration = TimeSpan.FromMinutes(cacheOpts.LocalCacheExpirationMinutes),
				Expiration = TimeSpan.FromMinutes(cacheOpts.CacheExpirationMinutes),
			};
		});
	}
}
