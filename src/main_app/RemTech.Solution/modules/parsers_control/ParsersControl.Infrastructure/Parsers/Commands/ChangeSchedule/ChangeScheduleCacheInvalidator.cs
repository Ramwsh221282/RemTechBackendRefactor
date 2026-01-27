using ParsersControl.Core.Features.ChangeSchedule;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.ChangeSchedule;

/// <summary>
/// Инвалидатор кэша для команды изменения расписания парсера.
/// </summary>
/// <param name="arrayInvalidator">Экземпляр инвалидатора кэша для массива парсеров.</param>
/// <param name="recordInvalidator">Экземпляр инвалидатора кэша для записи парсера.</param>
public sealed class ChangeScheduleCacheInvalidator(
	CachedParserArrayInvalidator arrayInvalidator,
	ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<ChangeScheduleCommand, SubscribedParser>
{
	/// <summary>
	/// Инвалидирует кэш после выполнения команды изменения расписания парсера.
	/// </summary>
	/// <param name="command">Команда изменения расписания парсера.</param>
	/// <param name="result">Результат выполнения команды - подписанный парсер.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	public Task InvalidateCache(ChangeScheduleCommand command, SubscribedParser result, CancellationToken ct = default)
	{
		Task[] tasks = [arrayInvalidator.Invalidate(ct), recordInvalidator.Invalidate(result, ct)];

		return Task.WhenAll(tasks);
	}
}
