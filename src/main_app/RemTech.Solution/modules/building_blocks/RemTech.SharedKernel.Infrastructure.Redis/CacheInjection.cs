using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace RemTech.SharedKernel.Infrastructure.Redis;

public static class CacheInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterHybridCache(IConfigurationManager configuration)
        {
            CachingOptions? cacheOptions = configuration.GetSection(nameof(CachingOptions)).Get<CachingOptions>();
            if (cacheOptions is null) 
                throw new InvalidOperationException("CachingOptions was not registered. Please register it using before calling AddHybridCache().");
            cacheOptions.Validate();
            RegisterCachingOptions(services, cacheOptions);
        }
        
        public void RegisterHybridCache()
        {
            CachingOptions? cacheOptions = GetCachingOptions(services);
            if (cacheOptions is null) 
                throw new InvalidOperationException("CachingOptions was not registered. Please register it using before calling AddHybridCache().");
            RegisterCachingOptions(services, cacheOptions);
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

    private static void RegisterCachingOptions(IServiceCollection services, CachingOptions cacheOptions)
    {
        cacheOptions.Validate();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = cacheOptions.RedisConnectionString;
        });
            
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions()
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(cacheOptions.LocalCacheExpirationMinutes),
                Expiration = TimeSpan.FromMinutes(cacheOptions.CacheExpirationMinutes),
            };
        });
    }
}