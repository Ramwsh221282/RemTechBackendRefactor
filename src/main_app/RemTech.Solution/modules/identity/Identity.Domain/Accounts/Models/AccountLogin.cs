using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Accounts.Models;

/// <summary>
/// Логин аккаунта.
/// </summary>
public sealed record AccountLogin
{
	private AccountLogin(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение логина аккаунта.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создает экземпляр <see cref="AccountLogin"/> с валидацией.
	/// </summary>
	/// <param name="value">Значение логина для создания.</param>
	/// <returns>Результат создания экземпляра <see cref="AccountLogin"/>.</returns>
	public static Result<AccountLogin> Create(string value) =>
		string.IsNullOrWhiteSpace(value) ? Error.Validation("Логин не может быть пустым.") : new AccountLogin(value);
}
