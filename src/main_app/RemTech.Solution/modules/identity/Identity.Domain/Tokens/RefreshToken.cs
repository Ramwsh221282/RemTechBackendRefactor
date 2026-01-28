using System.Security.Cryptography;
using Identity.Domain.Contracts.Jwt;

namespace Identity.Domain.Tokens;

/// <summary>
/// Представляет токен обновления.
/// </summary>
/// <param name="accountId">Идентификатор аккаунта.</param>
/// <param name="tokenValue">Значение токена.</param>
/// <param name="expiresAt">Время истечения срока действия токена в формате Unix времени.</param>
/// <param name="createdAt">Время создания токена в формате Unix времени.</param>
public sealed class RefreshToken(Guid accountId, string tokenValue, long expiresAt, long createdAt)
{
	/// <summary>
	/// Идентификатор аккаунта.
	/// </summary>
	public Guid AccountId { get; } = accountId;

	/// <summary>
	/// Значение токена.
	/// </summary>
	public string TokenValue { get; } = tokenValue;

	/// <summary>
	/// Время истечения срока действия токена в формате Unix времени.
	/// </summary>
	public long ExpiresAt { get; private set; } = expiresAt;

	/// <summary>
	/// Время создания токена в формате Unix времени.
	/// </summary>
	public long CreatedAt { get; private set; } = createdAt;

	/// <summary>
	/// Создает новый токен обновления.
	/// </summary>
	/// <param name="accountId">Идентификатор аккаунта.</param>
	/// <param name="expiresAt">Время истечения срока действия токена в формате Unix времени.</param>
	/// <param name="createdAt">Время создания токена в формате Unix времени.</param>
	/// <returns>Новый экземпляр токена обновления.</returns>
	public static RefreshToken CreateNew(Guid accountId, long expiresAt, long createdAt)
	{
		string tokenValue = GenerateRandomString();
		return new RefreshToken(accountId, tokenValue, expiresAt, createdAt);
	}

	/// <summary>
	/// Проверяет, истек ли срок действия токена обновления.
	/// </summary>
	/// <param name="tokenManager">Менеджер JWT токенов.</param>
	/// <returns>True, если срок действия токена истек; в противном случае false.</returns>
	public bool IsExpired(IJwtTokenManager tokenManager)
	{
		long created = tokenManager.GenerateRefreshToken(AccountId).CreatedAt;
		return IsExpired(created);
	}

	/// <summary>
	/// Проверяет, истек ли срок действия токена обновления.
	/// </summary>
	/// <param name="currentUnixTime">Текущее время в формате Unix времени.</param>
	/// <returns>True, если срок действия токена истек; в противном случае false.</returns>
	public bool IsExpired(long currentUnixTime) => currentUnixTime > ExpiresAt;

	private static string GenerateRandomString()
	{
		byte[] randomBytes = new byte[32];
		using RandomNumberGenerator rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomBytes);
		return Convert.ToBase64String(randomBytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
	}
}
