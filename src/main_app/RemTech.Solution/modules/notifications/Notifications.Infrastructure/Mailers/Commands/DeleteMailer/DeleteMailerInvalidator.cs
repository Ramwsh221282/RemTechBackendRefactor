using Notifications.Core.Mailers.Features.DeleteMailer;
using Notifications.Infrastructure.Mailers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace Notifications.Infrastructure.Mailers.Commands.DeleteMailer;

/// <summary>
/// Инвалидатор кэша для команды удаления почтового ящика.
/// </summary>
/// <param name="arrayCacheInvalidator">Инвалидатор кэша для массива почтовых ящиков.</param>
/// <param name="recordCacheInvalidator">Инвалидатор кэша для записи почтового ящика.</param>
public sealed class DeleteMailerInvalidator(
	MailerArrayCacheInvalidator arrayCacheInvalidator,
	MailerRecordCacheInvalidator recordCacheInvalidator
) : ICacheInvalidator<DeleteMailerCommand, Guid>
{
	/// <summary>
	/// Инвалидирует кэш после выполнения команды удаления почтового ящика.
	/// </summary>
	/// <param name="command">Команда удаления почтового ящика.</param>
	/// <param name="result">Идентификатор удаленного почтового ящика.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	public Task InvalidateCache(DeleteMailerCommand command, Guid result, CancellationToken ct = default)
	{
		Task[] tasks = [arrayCacheInvalidator.Invalidate(ct), recordCacheInvalidator.Invalidate(result, ct)];

		return Task.WhenAll(tasks);
	}
}
