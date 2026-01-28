using Identity.Domain.Accounts.Features.GivePermissions;
using Identity.Domain.Accounts.Models;
using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace Identity.Infrastructure.Accounts.Commands.GivePermissions;

/// <summary>
/// Инвалидатор кэша для команды предоставления разрешений.
/// </summary>
/// <param name="cache">Кэш для хранения данных.</param>
/// <param name="logger">Логгер для записи информации.</param>
public sealed class GivePermissionsCacheInvalidator(HybridCache cache, Serilog.ILogger logger)
	: ICacheInvalidator<GivePermissionsCommand, Account>
{
	private HybridCache Cache { get; } = cache;
	private Serilog.ILogger Logger { get; } = logger;

	/// <summary>
	/// Инвалидирует кэш после выполнения команды предоставления разрешений.
	/// </summary>
	/// <param name="command">Команда предоставления разрешений.</param>
	/// <param name="result">Результат выполнения команды.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
	public async Task InvalidateCache(GivePermissionsCommand command, Account result, CancellationToken ct = default)
	{
		string key = $"get_user_{result.Id.Value}";
		await Cache.RemoveAsync(key, ct);
		Logger.Information("Invalidated cache for key {Key}", key);
	}
}
