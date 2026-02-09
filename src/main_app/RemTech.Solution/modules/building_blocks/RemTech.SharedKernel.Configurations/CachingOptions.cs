namespace RemTech.SharedKernel.Configurations;

/// <summary>
/// Опции кэширования с использованием Redis.
/// </summary>
public sealed class CachingOptions
{
	/// <summary>
	/// Строка подключения к Redis.
	/// </summary>
	public string RedisConnectionString { get; set; } = string.Empty;

	/// <summary>
	/// Время истечения локального кэша в минутах.
	/// </summary>
	public int LocalCacheExpirationMinutes { get; set; }

	/// <summary>
	/// Время истечения кэша в Redis в минутах.
	/// </summary>
	public int CacheExpirationMinutes { get; set; }

	/// <summary>
	/// Валидировать настройки кэширования.
	/// </summary>
	/// <exception cref="InvalidOperationException">Выбрасывается, если настройки кэширования недействительны.</exception>
	public void Validate()
	{
		if (string.IsNullOrEmpty(RedisConnectionString))
		{
			throw new InvalidOperationException("RedisConnectionString is empty.");
		}
		if (LocalCacheExpirationMinutes <= 0)
		{
			throw new InvalidOperationException("LocalCacheExpirationMinutes must be greater than 0.");
		}
		if (CacheExpirationMinutes <= 0)
		{
			throw new InvalidOperationException("CacheExpirationMinutes must be greater than 0.");
		}
	}
}
