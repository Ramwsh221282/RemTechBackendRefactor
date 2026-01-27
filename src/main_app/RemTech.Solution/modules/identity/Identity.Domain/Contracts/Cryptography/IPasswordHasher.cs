using Identity.Domain.Accounts.Models;

namespace Identity.Domain.Contracts.Cryptography;

/// <summary>
/// Интерфейс для хеширования и проверки паролей аккаунтов.
/// </summary>
public interface IPasswordHasher
{
	/// <summary>
	/// Хеширует указанный пароль аккаунта.
	/// </summary>
	/// <param name="password">Пароль аккаунта для хеширования.</param>
	/// <returns>Хешированный пароль аккаунта.</returns>
	public AccountPassword Hash(AccountPassword password);

	/// <summary>
	/// Проверяет, соответствует ли введенное значение хешированному паролю.
	/// </summary>
	/// <param name="input">Введенное значение для проверки.</param>
	/// <param name="hashed">Хешированный пароль для сравнения.</param>
	/// <returns>True, если введенное значение соответствует хешированному паролю; в противном случае false.</returns>
	public bool Verify(string input, AccountPassword hashed);
}
