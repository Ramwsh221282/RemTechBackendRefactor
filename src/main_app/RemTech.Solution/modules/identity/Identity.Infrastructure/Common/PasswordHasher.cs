using BCrypt.Net;
using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Cryptography;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Common;

/// <summary>
/// Хешер паролей с использованием BCrypt.
/// </summary>
/// <param name="options">Настройки работы BCrypt.</param>
public sealed class PasswordHasher(IOptions<BcryptWorkFactorOptions> options) : IPasswordHasher
{
	private int WorkFactor { get; } = options.Value.WorkFactor;

	/// <summary>
	/// Хеширует пароль.
	/// </summary>
	/// <param name="password">Пароль для хеширования.</param>
	/// <returns>Хешированный пароль.</returns>
	public AccountPassword Hash(AccountPassword password)
	{
		string hashed = BCrypt.Net.BCrypt.EnhancedHashPassword(password.Value, HashType.SHA512, WorkFactor);
		return AccountPassword.Create(hashed);
	}

	/// <summary>
	/// Проверяет соответствие введенного пароля хешированному паролю.
	/// </summary>
	/// <param name="input">Введенный пароль.</param>
	/// <param name="hashed">Хешированный пароль.</param>
	/// <returns>Результат проверки соответствия.</returns>
	public bool Verify(string input, AccountPassword hashed) =>
		BCrypt.Net.BCrypt.EnhancedVerify(input, hashed.Value, HashType.SHA512);
}
