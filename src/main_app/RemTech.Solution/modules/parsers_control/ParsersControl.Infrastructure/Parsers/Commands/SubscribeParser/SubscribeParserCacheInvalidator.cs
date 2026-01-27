using ParsersControl.Core.Features.SubscribeParser;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.SubscribeParser;

/// <summary>
/// Инвалидатор кэша для команды подписки на парсер.
/// </summary>
/// <param name="arrayInvalidator">Экземпляр инвалидатора кэша для массива парсеров.</param>
/// <param name="recordInvalidator">Экземпляр инвалидатора кэша для записи парсера.</param>
public sealed class SubscribeParserCacheInvalidator(
	CachedParserArrayInvalidator arrayInvalidator,
	ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<SubscribeParserCommand, SubscribedParser>
{
	/// <summary>
	/// Инвалидирует кэш после выполнения команды подписки на парсер.
	/// </summary>
	/// <param name="command">Команда подписки на парсер.</param>
	/// <param name="result">Результат выполнения команды - подписанный парсер.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	public Task InvalidateCache(SubscribeParserCommand command, SubscribedParser result, CancellationToken ct = default)
	{
		Task[] tasks = [arrayInvalidator.Invalidate(ct), recordInvalidator.Invalidate(result, ct)];

		return Task.WhenAll(tasks);
	}
}
