namespace RemTech.SharedKernel.Infrastructure.Redis;

public sealed class CachingOptions
{
    public string RedisConnectionString { get; set; } = string.Empty;
    public int LocalCacheExpirationMinutes { get; set; }
    public int CacheExpirationMinutes { get; set; }

    public void Validate()
    {
        if (string.IsNullOrEmpty(RedisConnectionString))
            throw new InvalidOperationException("RedisConnectionString is empty.");
        if (LocalCacheExpirationMinutes <= 0)
            throw new InvalidOperationException("LocalCacheExpirationMinutes must be greater than 0.");
        if (CacheExpirationMinutes <= 0)
            throw new InvalidOperationException("CacheExpirationMinutes must be greater than 0.");
    }
}