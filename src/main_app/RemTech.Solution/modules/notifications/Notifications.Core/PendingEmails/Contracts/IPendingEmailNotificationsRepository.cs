namespace Notifications.Core.PendingEmails.Contracts;

/// <summary>
/// Контракт репозитория ожидающих email-уведомлений.
/// </summary>
public interface IPendingEmailNotificationsRepository
{
	/// <summary>
	/// Добавляет новое ожидающее email-уведомление в репозиторий.
	/// </summary>
	/// <param name="notification">Ожидающее email-уведомление для добавления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию добавления.</returns>
	Task Add(PendingEmailNotification notification, CancellationToken ct = default);

	/// <summary>
	/// Получает ожидающее email-уведомление из репозитория по заданной спецификации.
	/// </summary>
	/// <param name="spec">Спецификация для фильтрации ожидающих email-уведомлений.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию получения.</returns>
	Task<PendingEmailNotification[]> GetMany(
		PendingEmailNotificationsSpecification spec,
		CancellationToken ct = default
	);

	/// <summary>
	/// Удаляет множество ожидающих email-уведомлений из репозитория.
	/// </summary>
	/// <param name="notifications">Множество ожидающих email-уведомлений для удаления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию удаления, возвращающая количество удаленных элементов.</returns>
	Task<int> Remove(IEnumerable<PendingEmailNotification> notifications, CancellationToken ct = default);
}
