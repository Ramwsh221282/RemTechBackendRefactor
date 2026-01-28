using Identity.Domain.Contracts.Cryptography;
using Identity.Domain.PasswordRequirements;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Accounts.Models;

/// <summary>
/// Пароль аккаунта.
/// </summary>
public sealed record AccountPassword
{
	private AccountPassword(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение пароля аккаунта.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создает экземпляр <see cref="AccountPassword"/> с валидацией.
	/// </summary>
	/// <param name="value">Значение пароля для создания.</param>
	/// <returns>Результат создания экземпляра <see cref="AccountPassword"/>.</returns>
	public static Result<AccountPassword> Create(string value)
	{
		return string.IsNullOrWhiteSpace(value)
			? (Result<AccountPassword>)Error.Validation("Пароль не может быть пустым.")
			: (Result<AccountPassword>)new AccountPassword(value);
	}

	/// <summary>
	/// Хеширует пароль с помощью указанного хешера.
	/// </summary>
	/// <param name="hasher">Хешер для хеширования пароля.</param>
	/// <returns>Хешированный пароль.</returns>
	public AccountPassword HashBy(IPasswordHasher hasher) => hasher.Hash(this);

	/// <summary>
	/// Проверяет, соответствует ли введенное значение хешированному паролю с помощью указанного хешера.
	/// </summary>
	/// <param name="input">Введенное значение для проверки.</param>
	/// <param name="hasher">Хешер для проверки пароля.</param>
	/// <returns>Результат проверки соответствия.</returns>
	public bool Verify(string input, IPasswordHasher hasher) => hasher.Verify(input, this);

	/// <summary>
	/// Проверяет, удовлетворяет ли пароль указанному требованию.
	/// </summary>
	/// <param name="requirement">Требование к паролю для проверки.</param>
	/// <returns>Результат проверки соответствия.</returns>
	public Result<Unit> Satisfies(IAccountPasswordRequirement requirement) => requirement.Satisfies(this);
}
