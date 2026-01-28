using Notifications.Core.Mailers;
using Notifications.Core.Mailers.Features.ChangeCredentials;
using Notifications.Infrastructure.Mailers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace Notifications.Infrastructure.Mailers.Commands.ChangeCredentials;

/// <summary>
/// Инвалидатор кэша для команды изменения учетных данных почтового ящика.
/// </summary>
/// <param name="arrayCacheInvalidator">Инвалидатор кэша для массива почтовых ящиков.</param>
/// <param name="recordCacheInvalidator">Инвалидатор кэша для записи почтового ящика.</param>
public sealed class ChangeCredentialsCacheInvalidator(
	MailerArrayCacheInvalidator arrayCacheInvalidator,
	MailerRecordCacheInvalidator recordCacheInvalidator
) : ICacheInvalidator<ChangeCredentialsCommand, Mailer>
{
	/// <summary>
	/// Инвалидирует кэш после выполнения команды изменения учетных данных почтового ящика.
	/// </summary>
	/// <param name="command">Команда изменения учетных данных почтового ящика.</param>
	/// <param name="result">Результат выполнения команды, представляющий измененный почтовый ящик.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инвалидирования кэша.</returns>
	public Task InvalidateCache(ChangeCredentialsCommand command, Mailer result, CancellationToken ct = default)
	{
		Task[] tasks = [arrayCacheInvalidator.Invalidate(ct), recordCacheInvalidator.Invalidate(result.Id.Value, ct)];

		return Task.WhenAll(tasks);
	}
}
