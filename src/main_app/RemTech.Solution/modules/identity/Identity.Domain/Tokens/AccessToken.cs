namespace Identity.Domain.Tokens;

/// <summary>
/// Представляет токен доступа.
/// </summary>
public sealed class AccessToken
{
	/// <summary>
	/// Сырой токен доступа.
	/// </summary>
	public required string RawToken { get; set; }

	/// <summary>
	/// Идентификатор токена.
	/// </summary>
	public required Guid TokenId { get; set; }

	/// <summary>
	/// Время истечения срока действия токена в формате Unix времени.
	/// </summary>
	public required long ExpiresAt { get; set; }

	/// <summary>
	/// Время создания токена в формате Unix времени.
	/// </summary>
	public required long CreatedAt { get; set; }

	/// <summary>
	/// Электронная почта пользователя.
	/// </summary>
	public required string Email { get; set; }

	/// <summary>
	/// Логин пользователя.
	/// </summary>
	public required string Login { get; set; }

	/// <summary>
	/// Идентификатор пользователя.
	/// </summary>
	public required Guid UserId { get; set; }

	/// <summary>
	/// Разрешения, связанные с токеном доступа.
	/// </summary>
	public required IEnumerable<string> Permissions { get; set; }

	/// <summary>
	/// Сырой строковый список разрешений.
	/// </summary>
	public required string RawPermissionsString { get; set; }

	/// <summary>
	/// Флаг, указывающий, истек ли срок действия токена.
	/// </summary>
	public required bool IsExpired { get; set; }

	/// <summary>
	/// Проверяет, содержит ли токен указанное разрешение.
	/// </summary>
	/// <param name="permission">Разрешение для проверки.</param>
	/// <returns>True, если токен содержит указанное разрешение; в противном случае false.</returns>
	public bool ContainsPermission(string permission)
	{
		return RawPermissionsString.Contains(permission, StringComparison.OrdinalIgnoreCase);
	}
}
