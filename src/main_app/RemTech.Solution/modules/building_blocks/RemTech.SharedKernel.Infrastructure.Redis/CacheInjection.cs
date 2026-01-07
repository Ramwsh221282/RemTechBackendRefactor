using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace RemTech.SharedKernel.Infrastructure.Redis;

public static class CacheInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterHybridCache()
        {
            services.AddStackExchangeRedisCache(options =>
            {
                CachingOptions? cacheOptions = GetCachingOptions(services);
                if (cacheOptions is null) 
                    throw new InvalidOperationException("CachingOptions was not registered. Please register it using before calling AddHybridCache().");
                options.Configuration = cacheOptions.RedisConnectionString;
            });
            
            
            services.AddHybridCache(options =>
            {
                CachingOptions? cacheOptions = GetCachingOptions(services);
                if (cacheOptions is null) 
                    throw new InvalidOperationException("CachingOptions was not registered. Please register it using before calling AddHybridCache().");

                options.DefaultEntryOptions = new HybridCacheEntryOptions()
                {
                    LocalCacheExpiration = TimeSpan.FromMinutes(cacheOptions.LocalCacheExpirationMinutes),
                    Expiration = TimeSpan.FromMinutes(cacheOptions.CacheExpirationMinutes),
                };
            });
        }
    }

    private static CachingOptions? GetCachingOptions(IServiceCollection services)
    {
        ServiceDescriptor? descriptor = services
            .FirstOrDefault(s => s.ImplementationInstance != null &&
                                 s.ImplementationInstance.GetType() == typeof(OptionsWrapper<CachingOptions>));

        if (descriptor is null) return null;
        return ((OptionsWrapper<CachingOptions>)descriptor.ImplementationInstance!).Value;
    }
}