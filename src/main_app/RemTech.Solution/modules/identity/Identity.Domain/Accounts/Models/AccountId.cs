using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Accounts.Models;

/// <summary>
/// Идентификатор аккаунта.
/// </summary>
public readonly record struct AccountId
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="AccountId"/> со значением по умолчанию (новый GUID).
	/// </summary>
	public AccountId() => Value = Guid.NewGuid();

	private AccountId(Guid value) => Value = value;

	/// <summary>
	/// Значение идентификатора аккаунта.
	/// </summary>
	public Guid Value { get; }

	/// <summary>
	/// Создает новый уникальный идентификатор аккаунта.
	/// </summary>
	/// <returns>Новый уникальный идентификатор аккаунта.</returns>
	public static AccountId New() => new(Guid.NewGuid());

	/// <summary>
	/// Создает экземпляр <see cref="AccountId"/> с валидацией.
	/// </summary>
	/// <param name="value">Значение для создания идентификатора аккаунта.</param>
	/// <returns>Результат создания экземпляра <see cref="AccountId"/>.</returns>
	public static Result<AccountId> Create(Guid value) =>
		value == Guid.Empty
			? Error.Validation("Идентификатор учетной записи не может быть пустым.")
			: new AccountId(value);
}
