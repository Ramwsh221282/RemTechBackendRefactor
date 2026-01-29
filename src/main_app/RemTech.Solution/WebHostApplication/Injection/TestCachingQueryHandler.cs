using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;

namespace WebHostApplication.Injection;

// TODO move to shared kernel core.
// TODO: создать атрибут, который будет решать нужно ли облагать основной обработчик кеширующим декоратором (как в TransactionalHandlerDecorator). Поскольку не все запросы должны быть кешированными.

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

	private static readonly ConcurrentDictionary<Type, bool> _cachingAttributeCache = new();
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
		if (!HasCachingAttribute(Inner))
		{
			return await ReadFromOriginSource(query, ct);
		}

		string hashedPayload = ToSha256Hash(query.ToString());
		string key = $"{typeof(TQuery).Name}:{hashedPayload}";
		return await Cache.GetOrCreateAsync(
			key,
			async token => await Inner.Handle(query, token),
			options: _cacheOptions,
			cancellationToken: ct
		);
	}

	private Task<TResult> ReadFromOriginSource(TQuery query, CancellationToken ct) => Inner.Handle(query, ct);

	private static bool HasCachingAttribute(object? instance)
	{
		if (instance is null)
		{
			return false;
		}

		Type rootType = instance.GetType();
		return _cachingAttributeCache.GetOrAdd(
			rootType,
			static t =>
			{
				HashSet<Type> visited = [];
				return HasTransactionalAttributeForTypeRecursive(t, visited);
			}
		);
	}

	private static bool HasTransactionalAttributeForTypeRecursive(Type type, ISet<Type> visited)
	{
		if (!visited.Add(type))
		{
			return false;
		}

		if (type.GetCustomAttribute<UseCacheAttribute>() != null)
		{
			return true;
		}

		const BindingFlags flags =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

		foreach (FieldInfo field in type.GetFields(flags))
		{
			Type fieldType = field.FieldType;

			if (fieldType.GetCustomAttribute<UseCacheAttribute>() != null)
			{
				return true;
			}

			if (HasTransactionalAttributeForTypeRecursive(fieldType, visited))
			{
				return true;
			}
		}

		// тут начинается рекурсия по свойствам вложенного типа
		foreach (PropertyInfo prop in type.GetProperties(flags))
		{
			if (prop.GetIndexParameters().Length > 0)
			{
				continue;
			}

			Type propType = prop.PropertyType;

			if (propType.GetCustomAttribute<UseCacheAttribute>() != null)
			{
				return true;
			}

			if (HasTransactionalAttributeForTypeRecursive(propType, visited))
			{
				return true;
			}
		}

		return false;
	}
}

/// <summary>
/// Атрибут для указания использования кэша в обработчике запросов.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class UseCacheAttribute : Attribute;
