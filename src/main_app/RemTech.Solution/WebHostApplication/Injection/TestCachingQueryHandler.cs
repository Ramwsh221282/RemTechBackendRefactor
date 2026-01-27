using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;

namespace WebHostApplication.Injection;

// TODO move to shared kernel core.

/// <summary>
/// Обработчик запросов с кэшированием для использования в тестах.
/// </summary>
/// <typeparam name="TQuery">Тип запроса.</typeparam>
/// <typeparam name="TResult">Тип результата.</typeparam>
/// <param name="cache">Кэш для хранения результатов.</param>
/// <param name="inner">Внутренний обработчик запросов.</param>
public sealed class TestCachingQueryHandler<TQuery, TResult>(HybridCache cache, IQueryHandler<TQuery, TResult> inner)
	: ITestCachingQueryHandler<TQuery, TResult>
	where TQuery : IQuery
{
	private static HybridCacheEntryOptions _cacheOptions =>
		new() { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) };
	private HybridCache Cache { get; } = cache;
	private IQueryHandler<TQuery, TResult> Inner { get; } = inner;

	/// <summary>
	/// Обрабатывает запрос с использованием кэширования.
	/// </summary>
	/// <param name="query">Запрос для обработки.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Результат обработки запроса.</returns>
	public async Task<TResult> Handle(TQuery query, CancellationToken ct = default)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		TResult result = await ReadFromCache(query, ct);
		stopwatch.Stop();
		return result;
	}

	private static string ToSha256Hash(string input)
	{
		byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
		StringBuilder sb = new(bytes.Length * 2);
		foreach (byte b in bytes)
			sb.Append(b.ToString("x2"));
		return sb.ToString();
	}

	private async Task<TResult> ReadFromCache(TQuery query, CancellationToken ct)
	{
		string hashedPayload = ToSha256Hash(query.ToString());
		string key = $"{typeof(TQuery).Name}:{hashedPayload}";
		return await Cache.GetOrCreateAsync(
			key,
			async token => await Inner.Handle(query, token),
			options: _cacheOptions,
			cancellationToken: ct
		);
	}
}
