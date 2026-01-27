using ParsersControl.Core.Features.UpdateParserLink;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.UpdateParserLink;

/// <summary>
/// Инвалидатор кэша для команды обновления ссылки парсера.
/// </summary>
/// <param name="arrayInvalidator">Экземпляр инвалидатора кэша для массива парсеров.</param>
/// <param name="recordInvalidator">Экземпляр инвалидатора кэша для записи парсера.</param>
public sealed class UpdateParserLinkCacheInvalidator(
	CachedParserArrayInvalidator arrayInvalidator,
	ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<UpdateParserLinkCommand, SubscribedParserLink>
{
	/// <summary>
	/// Инвалидирует кэш после выполнения команды обновления ссылки парсера.
	/// </summary>
	/// <param name="command">Команда обновления ссылки парсера.</param>
	/// <param name="result">Результат выполнения команды - подписанная ссылка парсера.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	public Task InvalidateCache(
		UpdateParserLinkCommand command,
		SubscribedParserLink result,
		CancellationToken ct = default
	)
	{
		Task[] tasks = [arrayInvalidator.Invalidate(ct), recordInvalidator.Invalidate(result.ParserId, ct)];

		return Task.WhenAll(tasks);
	}
}
