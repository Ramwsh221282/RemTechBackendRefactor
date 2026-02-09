using Identity.Domain.Accounts.Models;
using Identity.Domain.Tokens;
using Microsoft.IdentityModel.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Jwt;

/// <summary>
/// Интерфейс для управления JWT-токенами.
/// </summary>
public interface IJwtTokenManager
{
	/// <summary>
	/// Генерирует токен доступа для указанного аккаунта.
	/// </summary>
	/// <param name="account">Аккаунт, для которого генерируется токен.</param>
	/// <returns>Сгенерированный токен доступа.</returns>
	AccessToken GenerateToken(Account account);

	/// <summary>
	/// Считывает токен из строки.
	/// </summary>
	/// <param name="tokenString">Строковое представление токена.</param>
	/// <returns>Считанный токен доступа.</returns>
	AccessToken ReadToken(string tokenString);

	/// <summary>
	/// Проверяет валидность JWT-токена и возвращает результат проверки.
	/// </summary>
	/// <param name="jwtToken">JWT-токен для проверки.</param>
	/// <returns>Результат проверки токена.</returns>
	Task<Result<TokenValidationResult>> GetValidToken(string jwtToken);

	/// <summary>
	/// Генерирует рефреш-токен для указанного аккаунта.
	/// </summary>
	/// <param name="accountId">Идентификатор аккаунта.</param>
	/// <returns>Сгенерированный рефреш-токен.</returns>
	RefreshToken GenerateRefreshToken(Guid accountId);
}
