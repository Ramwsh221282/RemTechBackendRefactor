using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace RemTech.SharedKernel.Infrastructure.Redis;

public sealed class CachingQueryHandler<TQuery, TResult> : ICachingQueryHandler<TQuery, TResult>
	where TQuery : IQuery
{
	private readonly HybridCacheEntryOptions _cacheOptions;
	private readonly HybridCache _cache;
	private readonly Serilog.ILogger _logger;
	private readonly IQueryHandler<TQuery, TResult> _inner;
	private static readonly ConcurrentDictionary<Type, bool> _cachingAttributeCache = [];

	public CachingQueryHandler(
		IOptions<CachingOptions> options,
		IQueryHandler<TQuery, TResult> inner,
		Serilog.ILogger logger,
		HybridCache cache
	)
	{
		_inner = inner;
		_logger = logger.ForContext<TQuery>();
		_cacheOptions = new()
		{
			Expiration = TimeSpan.FromMinutes(options.Value.CacheExpirationMinutes),
			LocalCacheExpiration = TimeSpan.FromMinutes(options.Value.LocalCacheExpirationMinutes),
		};
		_cache = cache;
	}

	public async Task<TResult> Handle(TQuery query, CancellationToken ct = default)
	{
		return HasCachingAttribute(_inner) switch
		{
			true => await ReadFromCache(query, ct),
			false => await _inner.Handle(query, ct),
		};
	}

	private async Task<TResult> ReadFromCache(TQuery query, CancellationToken ct = default)
	{
		_logger.Information("Handling as query in cache.");

		string hashedPayload = ToSha256Hash(query.ToString()!);
		string key = $"{typeof(TQuery).Name}:{hashedPayload}";

		return await _cache.GetOrCreateAsync(
			key,
			async token => await _inner.Handle(query, token),
			options: _cacheOptions,
			cancellationToken: ct
		);
	}

	private static bool HasCachingAttribute(IQueryHandler<TQuery, TResult> handler)
	{
		return _cachingAttributeCache.GetOrAdd(handler.GetType(), HasCachingAttributeRecursive);
	}

	private static string ToSha256Hash(string input)
	{
		byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
		StringBuilder sb = new(bytes.Length * 2);

		foreach (byte b in bytes)
		{
			sb.Append(b.ToString("x2"));
		}

		return sb.ToString();
	}

	private static bool HasCachingAttributeRecursive(Type type)
	{
		HashSet<Type> visited = [];
		return UseCacheAttribute.ObjectContainsAttribute(type, visited);
	}
}
