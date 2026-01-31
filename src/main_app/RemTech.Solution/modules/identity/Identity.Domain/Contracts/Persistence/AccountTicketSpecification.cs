namespace Identity.Domain.Contracts.Persistence;

/// <summary>
/// Спецификация для поиска тикетов аккаунтов.
/// </summary>
public sealed class AccountTicketSpecification
{
	/// <summary>
	/// Идентификатор аккаунта.
	/// </summary>
	public Guid? AccountId { get; private set; }

	/// <summary>
	/// Идентификатор тикета.
	/// </summary>
	public Guid? TicketId { get; private set; }

	/// <summary>
	/// Назначение тикета.
	/// </summary>
	public string? Purpose { get; private set; }

	/// <summary>
	/// Флаг, указывающий на то, что тикет завершен.
	/// </summary>
	public bool? Finished { get; private set; }

	/// <summary>
	/// Флаг, указывающий на необходимость блокировки тикета при выборке.
	/// </summary>
	public bool? LockRequired { get; private set; }

	/// <summary>
	/// Устанавливает идентификатор аккаунта для спецификации.
	/// </summary>
	/// <param name="accountId">Идентификатор аккаунта.</param>
	/// <returns>Обновленная спецификация тикета аккаунта.</returns>
	public AccountTicketSpecification WithAccountId(Guid accountId)
	{
		if (AccountId.HasValue)
		{
			return this;
		}

		AccountId = accountId;
		return this;
	}

	/// <summary>
	/// Устанавливает идентификатор тикета для спецификации.
	/// </summary>
	/// <param name="ticketId">Идентификатор тикета.</param>
	/// <returns>Обновленная спецификация тикета аккаунта.</returns>
	public AccountTicketSpecification WithTicketId(Guid ticketId)
	{
		if (TicketId.HasValue)
		{
			return this;
		}

		TicketId = ticketId;
		return this;
	}

	/// <summary>
	/// Устанавливает назначение тикета для спецификации.
	/// </summary>
	/// <param name="purpose">Назначение тикета.</param>
	/// <returns>Обновленная спецификация тикета аккаунта.</returns>
	public AccountTicketSpecification WithPurpose(string purpose)
	{
		if (!string.IsNullOrWhiteSpace(Purpose))
		{
			return this;
		}

		Purpose = purpose;
		return this;
	}

	/// <summary>
	/// Устанавливает флаг необходимости блокировки тикета при выборке.
	/// </summary>
	/// <returns>Обновленная спецификация тикета аккаунта.</returns>
	public AccountTicketSpecification WithLockRequired()
	{
		if (LockRequired.HasValue)
		{
			return this;
		}

		LockRequired = true;
		return this;
	}

	/// <summary>
	/// Устанавливает флаг, указывающий на то, что тикет не завершен.
	/// </summary>
	/// <returns>Обновленная спецификация тикета аккаунта.</returns>
	public AccountTicketSpecification NotFinished()
	{
		if (Finished.HasValue)
		{
			return this;
		}

		Finished = false;
		return this;
	}

	/// <summary>
	/// Устанавливает флаг, указывающий на то, что тикет завершен.
	/// </summary>
	/// <returns>Обновленная спецификация тикета аккаунта.</returns>
	public AccountTicketSpecification WithFinished()
	{
		if (Finished.HasValue)
		{
			return this;
		}

		Finished = true;
		return this;
	}
}
