using Notifications.Core.Mailers;
using Notifications.Core.PendingEmails;

namespace Notifications.Core.Common.Contracts;

/// <summary>
/// Единица работы модуля уведомлений.
/// </summary>
public interface INotificationsModuleUnitOfWork
{
	/// <summary>
	/// Сохраняет изменения в коллекции почтовых рассылок и ожидающих email-уведомлений.
	/// </summary>
	/// <param name="mailers">Коллекция почтовых рассылок для сохранения.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию сохранения.</returns>
	Task Save(IEnumerable<Mailer> mailers, CancellationToken ct = default);

	/// <summary>
	/// Сохраняет изменения в почтовой рассылке.
	/// </summary>
	/// <param name="mailer">Почтовая рассылка для сохранения.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию сохранения.</returns>
	Task Save(Mailer mailer, CancellationToken ct = default);

	/// <summary>
	/// Сохраняет изменения в коллекции ожидающих email-уведомлений.
	/// </summary>
	/// <param name="notifications">Коллекция ожидающих email-уведомлений для сохранения.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию сохранения.</returns>
	Task Save(IEnumerable<PendingEmailNotification> notifications, CancellationToken ct = default);

	/// <summary>
	/// Отслеживает изменения в коллекции почтовых рассылок.
	/// </summary>
	/// <param name="mailers">Коллекция почтовых рассылок для отслеживания.</param>
	void Track(IEnumerable<Mailer> mailers);

	/// <summary>
	/// Отслеживает изменения в коллекции ожидающих email-уведомлений.
	/// </summary>
	/// <param name="notifications">Коллекция ожидающих email-уведомлений для отслеживания.</param>
	void Track(IEnumerable<PendingEmailNotification> notifications);
}
