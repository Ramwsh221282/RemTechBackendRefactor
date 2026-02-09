namespace RemTech.SharedKernel.Configurations;

/// <summary>
/// Настройки для JWT.
/// </summary>
public sealed class JwtOptions
{
	/// <summary>
	/// Секретный ключ для подписи JWT.
	/// </summary>
	public string SecretKey { get; set; } = string.Empty;

	/// <summary>
	/// Издатель JWT.
	/// </summary>
	public string Issuer { get; set; } = string.Empty;

	/// <summary>
	/// Аудитория JWT.
	/// </summary>
	public string Audience { get; set; } = string.Empty;

	/// <summary>
	/// Проверка строки ключа.
	/// </summary>
	/// <exception cref="InvalidOperationException">Ошибка валидации ключа.</exception>
	public void Validate()
	{
		if (string.IsNullOrEmpty(SecretKey))
		{
			throw new InvalidOperationException("Secret key must not be empty.");
		}

		if (string.IsNullOrEmpty(Issuer))
		{
			throw new InvalidOperationException("Issuer must not be empty.");
		}

		if (string.IsNullOrEmpty(Audience))
		{
			throw new InvalidOperationException("Audience must not be empty.");
		}
	}
}
