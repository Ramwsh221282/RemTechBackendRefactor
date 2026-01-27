using ParsersControl.Core.Features.EnableParser;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.EnableParser;

/// <summary>
/// Инвалидатор кэша для команды включения парсера.
/// </summary>
/// <param name="arrayInvalidator">Экземпляр инвалидатора кэша для массива парсеров.</param>
/// <param name="recordInvalidator">Экземпляр инвалидатора кэша для записи парсера.</param>
public sealed class EnableParserCacheInvalidator(
	CachedParserArrayInvalidator arrayInvalidator,
	ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<EnableParserCommand, SubscribedParser>
{
	/// <summary>
	/// Инвалидирует кэш после выполнения команды включения парсера.
	/// </summary>
	/// <param name="command">Команда включения парсера.</param>
	/// <param name="result">Результат выполнения команды - подписанный парсер.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	public Task InvalidateCache(EnableParserCommand command, SubscribedParser result, CancellationToken ct = default)
	{
		Task[] tasks = [arrayInvalidator.Invalidate(ct), recordInvalidator.Invalidate(result, ct)];

		return Task.WhenAll(tasks);
	}
}
