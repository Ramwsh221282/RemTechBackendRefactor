namespace RemTech.SharedKernel.Infrastructure.Redis;

public sealed class CachingOptions
{
    public string RedisConnectionString { get; set; } = string.Empty;
    public int LocalCacheExpirationMinutes { get; set; }
    public int CacheExpirationMinutes { get; set; }
}