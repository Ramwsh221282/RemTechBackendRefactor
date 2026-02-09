using System.Text.RegularExpressions;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Accounts.Models;

/// <summary>
/// Электронная почта аккаунта.
/// </summary>
public sealed record AccountEmail
{
	private static readonly Regex _emailRegex = new(
		@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
		RegexOptions.Compiled
	);

	private const int MAX_EMAIL_LENGTH = 256;

	private AccountEmail(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение электронной почты аккаунта.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создает экземпляр <see cref="AccountEmail"/> с валидацией.
	/// </summary>
	/// <param name="value">Значение электронной почты для создания.</param>
	/// <returns>Результат создания экземпляра <see cref="AccountEmail"/>.</returns>
	public static Result<AccountEmail> Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return Error.Validation("Email не может быть пустым.");
		}

		if (value.Length > MAX_EMAIL_LENGTH)
		{
			return Error.Validation($"Email не может быть длиннее {MAX_EMAIL_LENGTH} символов.");
		}

		return !_emailRegex.IsMatch(value)
			? (Result<AccountEmail>)Error.InvalidFormat($"Email: {value} некоррекнтого формата.")
			: (Result<AccountEmail>)new AccountEmail(value);
	}
}
