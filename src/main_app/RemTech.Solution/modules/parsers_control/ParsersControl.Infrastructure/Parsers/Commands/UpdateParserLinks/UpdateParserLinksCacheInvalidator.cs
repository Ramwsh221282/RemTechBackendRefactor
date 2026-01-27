using ParsersControl.Core.Features.UpdateParserLinks;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.UpdateParserLinks;

/// <summary>
/// Инвалидатор кэша для команды обновления ссылок парсера.
/// </summary>
/// <param name="arrayInvalidator">Экземпляр инвалидатора кэша для массива парсеров.</param>
/// <param name="recordInvalidator">Экземпляр инвалидатора кэша для записи парсера.</param>
public sealed class UpdateParserLinksCacheInvalidator(
	CachedParserArrayInvalidator arrayInvalidator,
	ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<UpdateParserLinksCommand, IEnumerable<SubscribedParserLink>>
{
	/// <summary>
	/// 	Инвалидирует кэш после выполнения команды обновления ссылок парсера.
	/// </summary>
	/// <param name="command">Команда обновления ссылок парсера.</param>
	/// <param name="result">Результат выполнения команды - коллекция подписанных ссылок парсера.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	public Task InvalidateCache(
		UpdateParserLinksCommand command,
		IEnumerable<SubscribedParserLink> result,
		CancellationToken ct = default
	)
	{
		Task[] tasks =
		[
			arrayInvalidator.Invalidate(ct),
			Task.WhenAll(result.Select(link => recordInvalidator.Invalidate(link.ParserId, ct))),
		];

		return Task.WhenAll(tasks);
	}
}
