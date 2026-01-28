using ParsersControl.Core.Features.PermantlyDisableManyParsing;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.PermantlyDisableManyParsing;

/// <summary>
/// Инвалидатор кэша для команды постоянного отключения множества парсеров.
/// </summary>
/// <param name="arrayInvalidator">Экземпляр инвалидатора кэша для массива парсеров.</param>
/// <param name="recordInvalidator">Экземпляр инвалидатора кэша для записи парсера.</param>
public sealed class PermantlyDisableManyParsingCacheInvalidator(
	CachedParserArrayInvalidator arrayInvalidator,
	ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<PermantlyDisableManyParsingCommand, IEnumerable<SubscribedParser>>
{
	/// <summary>
	/// Инвалидирует кэш после выполнения команды постоянного отключения множества парсеров.
	/// </summary>
	/// <param name="command">Команда постоянного отключения множества парсеров.</param>
	/// <param name="result">Результат выполнения команды - коллекция подписанных парсеров.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	public Task InvalidateCache(
		PermantlyDisableManyParsingCommand command,
		IEnumerable<SubscribedParser> result,
		CancellationToken ct = default
	)
	{
		IEnumerable<Task> recordInvalidationTasks =
		[
			arrayInvalidator.Invalidate(ct),
			.. result.Select(p => recordInvalidator.Invalidate(p, ct)),
		];

		return Task.WhenAll(recordInvalidationTasks);
	}
}
