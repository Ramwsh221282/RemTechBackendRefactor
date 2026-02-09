namespace Identity.Domain.Accounts.Models;

/// <summary>
/// Статус активации аккаунта.
/// </summary>
public readonly record struct AccountActivationStatus
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="AccountActivationStatus"/> со значением по умолчанию (неактивирован).
	/// </summary>
	public AccountActivationStatus()
	{
		Value = false;
	}

	/// <summary>
	/// Значение статуса активации аккаунта.
	/// </summary>
	public bool Value { get; private init; }

	/// <summary>
	/// Создает статус активации "неактивирован".
	/// </summary>
	/// <returns>Статус активации "неактивирован".</returns>
	public static AccountActivationStatus NotActivated()
	{
		return new();
	}

	/// <summary>
	/// Создает статус активации "активирован".
	/// </summary>
	/// <returns>Статус активации "активирован".</returns>
	public static AccountActivationStatus Activated()
	{
		return new() { Value = true };
	}

	/// <summary>
	/// Создает статус активации на основе булевого значения.
	/// </summary>
	/// <param name="value">Значение для создания статуса активации.</param>
	/// <returns>Статус активации на основе булевого значения.</returns>
	public static AccountActivationStatus Create(bool value)
	{
		return new() { Value = value };
	}

	/// <summary>
	/// Проверяет, активирован ли аккаунт.
	/// </summary>
	/// <returns>True, если аккаунт активирован; в противном случае false.</returns>
	public bool IsActivated()
	{
		return Value;
	}
}
