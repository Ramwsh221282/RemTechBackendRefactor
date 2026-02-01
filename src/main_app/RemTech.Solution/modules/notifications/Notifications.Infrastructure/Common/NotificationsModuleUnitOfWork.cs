using Notifications.Core.Common.Contracts;
using Notifications.Core.Mailers;
using Notifications.Core.PendingEmails;
using Notifications.Infrastructure.Mailers;
using Notifications.Infrastructure.PendingEmails;

namespace Notifications.Infrastructure.Common;

/// <summary>
/// Единица работы модуля уведомлений.
/// </summary>
/// <param name="mailers">Трекер изменений для почтовых ящиков.</param>
/// <param name="pendingEmails">Трекер изменений для ожидающих email-уведомлений.</param>
public sealed class NotificationsModuleUnitOfWork(
	MailersChangeTracker mailers,
	PendingEmailsChangeTracker pendingEmails
) : INotificationsModuleUnitOfWork
{
	private MailersChangeTracker Mailers { get; } = mailers;

	/// <summary>
	/// Трекер изменений для ожидающих email-уведомлений.
	/// </summary>
	/// <param name="mailers">Коллекция почтовых ящиков для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию сохранения.</returns>
	public Task Save(IEnumerable<Mailer> mailers, CancellationToken ct = default)
	{
		return Mailers.Save(mailers, ct);
	}

	/// <summary>
	/// Сохраняет изменения почтового ящика.
	/// </summary>
	/// <param name="mailer">Почтовый ящик для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию сохранения.</returns>
	public Task Save(Mailer mailer, CancellationToken ct = default)
	{
		return Mailers.Save([mailer], ct);
	}

	/// <summary>
	/// Сохраняет изменения ожидающих email-уведомлений.
	/// </summary>
	/// <param name="notifications">Коллекция ожидающих email-уведомлений для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию сохранения.</returns>
	public Task Save(IEnumerable<PendingEmailNotification> notifications, CancellationToken ct = default)
	{
		return pendingEmails.Save(notifications, ct);
	}

	/// <summary>
	/// Отслеживает изменения почтовых ящиков.
	/// </summary>
	/// <param name="mailers">Коллекция почтовых ящиков для отслеживания.</param>
	public void Track(IEnumerable<Mailer> mailers)
	{
		Mailers.Track(mailers);
	}

	/// <summary>
	/// Отслеживает изменения ожидающих email-уведомлений.
	/// </summary>
	/// <param name="notifications">Коллекция ожидающих email-уведомлений для отслеживания.</param>
	public void Track(IEnumerable<PendingEmailNotification> notifications)
	{
		
	}
}
