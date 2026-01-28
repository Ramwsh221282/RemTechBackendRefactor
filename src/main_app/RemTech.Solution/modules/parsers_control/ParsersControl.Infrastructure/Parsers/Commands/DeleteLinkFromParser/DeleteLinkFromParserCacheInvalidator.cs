using ParsersControl.Core.Features.DeleteLinkFromParser;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.DeleteLinkFromParser;

/// <summary>
/// Инвалидатор кэша для команды удаления ссылки из парсера.
/// </summary>
/// <param name="arrayInvalidator">Экземпляр инвалидатора кэша для массива парсеров.</param>
/// <param name="recordInvalidator">Экземпляр инвалидатора кэша для записи парсера.</param>
public sealed class DeleteLinkFromParserCacheInvalidator(
	CachedParserArrayInvalidator arrayInvalidator,
	ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<DeleteLinkFromParserCommand, SubscribedParserLink>
{
	/// <summary>
	/// Инвалидирует кэш после выполнения команды удаления ссылки из парсера.
	/// </summary>
	/// <param name="command">Команда удаления ссылки из парсера.</param>
	/// <param name="result">Результат выполнения команды - подписанная ссылка на парсер.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	public Task InvalidateCache(
		DeleteLinkFromParserCommand command,
		SubscribedParserLink result,
		CancellationToken ct = default
	)
	{
		Task[] tasks = [arrayInvalidator.Invalidate(ct), recordInvalidator.Invalidate(command.ParserId, ct)];

		return Task.WhenAll(tasks);
	}
}
