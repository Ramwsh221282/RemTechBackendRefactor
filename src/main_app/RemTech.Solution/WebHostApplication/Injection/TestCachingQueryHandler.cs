using System.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;

namespace WebHostApplication.Injection;

// TODO move to shared kernel core.
public sealed class TestCachingQueryHandler<TQuery, TResult>(HybridCache cache, IQueryHandler<TQuery, TResult> inner)
	: ITestCachingQueryHandler<TQuery, TResult>
	where TQuery : IQuery
{
	private static HybridCacheEntryOptions _cacheOptions =>
		new() { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) };
	private HybridCache Cache { get; } = cache;
	private IQueryHandler<TQuery, TResult> Inner { get; } = inner;

	public async Task<TResult> Handle(TQuery query, CancellationToken ct = default)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		TResult result = await ReadFromCache(query, ct);
		stopwatch.Stop();
		return result;
	}

	private async Task<TResult> ReadFromCache(TQuery query, CancellationToken ct)
	{
		string queryPayload = query.ToString();
		string key = $"{nameof(TQuery)}_{queryPayload}";
		return await Cache.GetOrCreateAsync(
			key,
			async token => await Inner.Handle(query, token),
			options: _cacheOptions,
			cancellationToken: ct
		);
	}
}
