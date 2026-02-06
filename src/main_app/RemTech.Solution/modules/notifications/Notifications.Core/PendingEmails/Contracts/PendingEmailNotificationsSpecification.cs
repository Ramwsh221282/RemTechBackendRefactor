namespace Notifications.Core.PendingEmails.Contracts;

/// <summary>
/// Спецификация для фильтрации ожидающих email-уведомлений.
/// </summary>
public sealed class PendingEmailNotificationsSpecification
{
	/// <summary>
	/// Фильтровать только отправленные уведомления.
	/// </summary>
	public bool? SentOnly { get; private set; }

	/// <summary>
	/// Фильтровать только неотправленные уведомления.
	/// </summary>
	public bool? NotSentOnly { get; private set; }

	/// <summary>
	/// Требуется ли блокировка уведомлений.
	/// </summary>
	public bool? LockRequired { get; private set; }

	/// <summary>
	/// Лимит количества уведомлений для получения.
	/// </summary>
	public int? Limit { get; private set; }

	/// <summary>
	/// Фильтрует только отправленные уведомления.
	/// </summary>
	/// <returns>С фильтром только отправленных уведомлений.</returns>
	public PendingEmailNotificationsSpecification OfSentOnly()
	{
		if (SentOnly.HasValue)
		{
			return this;
		}

		SentOnly = true;
		return this;
	}

	/// <summary>
	/// Фильтрует только неотправленные уведомления.
	/// </summary>
	/// <returns>С фильтром только неотправленных уведомлений.</returns>
	public PendingEmailNotificationsSpecification OfNotSentOnly()
	{
		if (NotSentOnly.HasValue)
		{
			return this;
		}

		NotSentOnly = true;
		return this;
	}

	/// <summary>
	/// Требует блокировки уведомлений.
	/// </summary>
	/// <returns>С требованием блокировки уведомлений.</returns>
	public PendingEmailNotificationsSpecification WithLock()
	{
		if (LockRequired.HasValue)
		{
			return this;
		}

		LockRequired = true;
		return this;
	}

	/// <summary>
	/// Устанавливает лимит количества уведомлений для получения.
	/// </summary>
	/// <param name="limit">Ограничение при выборке уведомлений.</param>
	/// <returns>С установленным лимитом количества уведомлений для получения.</returns>
	public PendingEmailNotificationsSpecification WithLimit(int limit)
	{
		if (Limit.HasValue)
		{
			return this;
		}

		Limit = limit;
		return this;
	}
}
