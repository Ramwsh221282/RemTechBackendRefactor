using ParsersControl.Core.Features.AddParserLink;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.AddParserLink;

/// <summary>
/// Инвалидатор кэша для команды добавления ссылки на парсер.
/// </summary>
/// <param name="invalidator">Экземпляр инвалидатора кэша для массива парсеров.</param>
/// <param name="cacheRecordInvalidator">Экземпляр инвалидатора кэша для записи парсера.</param>
public sealed class AddParserLinkCacheInvalidator(
	CachedParserArrayInvalidator invalidator,
	ParserCacheRecordInvalidator cacheRecordInvalidator
) : ICacheInvalidator<AddParserLinkCommand, IEnumerable<SubscribedParserLink>>
{
	/// <summary>
	/// Инвалидирует кэш после выполнения команды добавления ссылки на парсер.
	/// </summary>
	/// <param name="command">Команда добавления ссылки на парсер.</param>
	/// <param name="result">Результат выполнения команды - коллекция подписанных ссылок на парсер.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	public Task InvalidateCache(
		AddParserLinkCommand command,
		IEnumerable<SubscribedParserLink> result,
		CancellationToken ct = default
	)
	{
		Task[] tasks = [invalidator.Invalidate(ct), cacheRecordInvalidator.Invalidate(command.ParserId, ct)];

		return Task.WhenAll(tasks);
	}
}
