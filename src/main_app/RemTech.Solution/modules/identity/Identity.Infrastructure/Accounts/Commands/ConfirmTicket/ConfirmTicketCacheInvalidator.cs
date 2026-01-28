using Identity.Domain.Accounts.Features.ConfirmTicket;
using Identity.Domain.Accounts.Models;
using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace Identity.Infrastructure.Accounts.Commands.ConfirmTicket;

/// <summary>
/// Инвалидатор кэша для команды подтверждения тикета.
/// </summary>
/// <param name="logger">Логгер для записи информации.</param>
/// <param name="cache">Кэш для хранения данных.</param>
public sealed class ConfirmTicketCacheInvalidator(Serilog.ILogger logger, HybridCache cache)
	: ICacheInvalidator<ConfirmTicketCommand, Account>
{
	private Serilog.ILogger Logger { get; } = logger.ForContext<ConfirmTicketCacheInvalidator>();
	private HybridCache Cache { get; } = cache;

	/// <summary>
	/// Инвалидирует кэш после выполнения команды подтверждения тикета.
	/// </summary>
	/// <param name="command">Команда подтверждения тикета.</param>
	/// <param name="result">Результат выполнения команды.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
	public async Task InvalidateCache(ConfirmTicketCommand command, Account result, CancellationToken ct = default)
	{
		string key = $"get_user_{result.Id.Value}";
		await Cache.RemoveAsync(key, ct);
		Logger.Information("Invalidated cache for key {Key}", key);
	}
}
