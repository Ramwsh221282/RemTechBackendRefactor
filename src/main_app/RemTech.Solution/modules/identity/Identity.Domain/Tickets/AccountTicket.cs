using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Tickets;

/// <summary>
/// Заявка аккаунта.
/// </summary>
/// <param name="accountId">Идентификатор аккаунта.</param>
/// <param name="ticketId">Идентификатор заявки.</param>
/// <param name="finished">Статус выполнения заявки.</param>
/// <param name="purpose">Причина создания заявки.</param>
public sealed class AccountTicket(AccountId accountId, Guid ticketId, bool finished, string purpose)
{
	private AccountTicket(AccountTicket ticket)
		: this(ticket.AccountId, ticket.TicketId, ticket.Finished, ticket.Purpose) { }

	/// <summary>
	/// Идентификатор аккаунта.
	/// </summary>
	public AccountId AccountId { get; } = accountId;

	/// <summary>
	/// Идентификатор заявки.
	/// </summary>
	public Guid TicketId { get; } = ticketId;

	/// <summary>
	/// Статус выполнения заявки.
	/// </summary>
	public bool Finished { get; private set; } = finished;

	/// <summary>
	/// Причина создания заявки.
	/// </summary>
	public string Purpose { get; } = purpose;

	/// <summary>
	/// Создает новую заявку аккаунта с валидацией.
	/// </summary>
	/// <param name="accountId">Идентификатор аккаунта.</param>
	/// <param name="purpose">Причина создания заявки.</param>
	/// <returns>Результат создания заявки аккаунта.</returns>
	public static Result<AccountTicket> New(Guid accountId, string purpose)
	{
		return string.IsNullOrWhiteSpace(purpose)
			? Error.Validation("Причина создания заявки не указана.")
			: new AccountTicket(AccountId.Create(accountId), Guid.NewGuid(), false, purpose);
	}

	/// <summary>
	/// Создает новую заявку аккаунта с валидацией.
	/// </summary>
	/// <param name="accountId">Идентификатор аккаунта.</param>
	/// <param name="purpose">Причина создания заявки.</param>
	/// <returns>Результат создания заявки аккаунта.</returns>
	public static Result<AccountTicket> New(AccountId accountId, string purpose) => New(accountId.Value, purpose);

	/// <summary>
	/// Создает новую заявку аккаунта с валидацией.
	/// </summary>
	/// <param name="account">Аккаунт.</param>
	/// <param name="purpose">Причина создания заявки.</param>
	/// <returns>Результат создания заявки аккаунта.</returns>
	public static Result<AccountTicket> New(Account account, string purpose) => New(account.Id, purpose);

	/// <summary>
	/// Завершает заявку.
	/// </summary>
	/// <returns>Результат завершения заявки.</returns>
	public Result<Unit> Finish()
	{
		if (Finished)
			return Error.Conflict("Заявка уже выполнена.");
		Finished = true;
		return Result.Success(Unit.Value);
	}

	/// <summary>
	/// Завершает заявку указанным идентификатором исполнителя.
	/// </summary>
	/// <param name="finisherId">Идентификатор исполнителя.</param>
	/// <returns>Результат завершения заявки.</returns>
	public Result<Unit> FinishBy(Guid finisherId)
	{
		if (Finished)
			return Error.Conflict("Заявка уже выполнена.");
		if (finisherId != AccountId.Value)
			return Error.Forbidden("Заявка не привязана к учетной записи.");
		Finished = true;
		return Result.Success(Unit.Value);
	}

	/// <summary>
	/// Клонирует заявку.
	/// </summary>
	/// <returns>Клон заявки.</returns>
	public AccountTicket Clone() => new(this);
}
