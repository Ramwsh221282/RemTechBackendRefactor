using ParsersControl.Core.Features.ChangeLinkActivity;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.ChangeLinkActivity;

/// <summary>
/// Инвалидатор кэша для команды изменения активности ссылки на парсер.
/// </summary>
/// <param name="arrayInvalidator">Экземпляр инвалидатора кэша для массива парсеров.</param>
/// <param name="recordInvalidator">Экземпляр инвалидатора кэша для записи парсера.</param>
public sealed class ChangeLinkActivityCacheInvalidator(
	CachedParserArrayInvalidator arrayInvalidator,
	ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<ChangeLinkActivityCommand, SubscribedParserLink>
{
	/// <summary>
	/// Инвалидирует кэш после выполнения команды изменения активности ссылки на парсер.
	/// </summary>
	/// <param name="command">Команда изменения активности ссылки на парсер.</param>
	/// <param name="result">Результат выполнения команды - подписанная ссылка на парсер.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	public Task InvalidateCache(
		ChangeLinkActivityCommand command,
		SubscribedParserLink result,
		CancellationToken ct = default
	)
	{
		Task[] tasks = [arrayInvalidator.Invalidate(ct), recordInvalidator.Invalidate(command.ParserId, ct)];

		return Task.WhenAll(tasks);
	}
}
