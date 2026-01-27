using Notifications.Core.Mailers;
using Notifications.Core.Mailers.Features.AddMailer;
using Notifications.Infrastructure.Mailers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace Notifications.Infrastructure.Mailers.Commands.AddMailer;

/// <summary>
/// Инвалидатор кэша для команды добавления почтового ящика.
/// </summary>
/// <param name="arrayInvalidator">Инвалидатор кэша для массива почтовых ящиков.</param>
/// <param name="recordInvalidator">Инвалидатор кэша для записи почтового ящика.</param>
public sealed class AddMailerCacheInvalidator(
	MailerArrayCacheInvalidator arrayInvalidator,
	MailerRecordCacheInvalidator recordInvalidator
) : ICacheInvalidator<AddMailerCommand, Mailer>
{
	/// <summary>
	/// Инвалидирует кэш после выполнения команды добавления почтового ящика.
	/// </summary>
	/// <param name="command">Команда добавления почтового ящика.</param>
	/// <param name="result">Результат выполнения команды, представляющий добавленный почтовый ящик.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	public Task InvalidateCache(AddMailerCommand command, Mailer result, CancellationToken ct = default)
	{
		Task[] tasks = [arrayInvalidator.Invalidate(ct), recordInvalidator.Invalidate(result.Id.Value, ct)];

		return Task.WhenAll(tasks);
	}
}
