using ParsersControl.Core.Features.SetLinkWorkTime;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.SetLinkWorkTime;

/// <summary>
/// Инвалидатор кэша для команды установки рабочего времени ссылки парсера.
/// </summary>
/// <param name="arrayInvalidator">Экземпляр инвалидатора кэша для массива парсеров.</param>
/// <param name="recordInvalidator">Экземпляр инвалидатора кэша для записи парсера.</param>
public sealed class SetLinkWorkTimeCacheInvalidator(
	CachedParserArrayInvalidator arrayInvalidator,
	ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<SetLinkWorkingTimeCommand, SubscribedParserLink>
{
	/// <summary>
	/// Инвалидирует кэш после выполнения команды установки рабочего времени ссылки парсера.
	/// </summary>
	/// <param name="command">Команда установки рабочего времени ссылки парсера.</param>
	/// <param name="result">Результат выполнения команды - подписанная ссылка парсера.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	public Task InvalidateCache(
		SetLinkWorkingTimeCommand command,
		SubscribedParserLink result,
		CancellationToken ct = default
	)
	{
		Task[] tasks = [arrayInvalidator.Invalidate(ct), recordInvalidator.Invalidate(command.ParserId, ct)];

		return Task.WhenAll(tasks);
	}
}
