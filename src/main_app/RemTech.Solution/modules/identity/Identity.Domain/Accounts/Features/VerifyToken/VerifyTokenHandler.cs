using Identity.Domain.Contracts.Jwt;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tokens;
using Microsoft.IdentityModel.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.VerifyToken;

/// <summary>
/// Обработчик команды для проверки токена пользователя.
/// </summary>
/// <param name="accessTokens">Репозиторий токенов доступа.</param>
/// <param name="tokensManager">Менеджер JWT токенов.</param>
public sealed class VerifyTokenHandler(IAccessTokensRepository accessTokens, IJwtTokenManager tokensManager)
	: ICommandHandler<VerifyTokenCommand, Unit>
{
	/// <summary>
	/// Выполняет проверку токена пользователя по команде.
	/// </summary>
	/// <param name="command">Команда проверки токена.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды.</returns>
	public async Task<Result<Unit>> Execute(VerifyTokenCommand command, CancellationToken ct = default)
	{
		Result<AccessToken> token = await accessTokens.Find(command.Token, withLock: true, ct);
		if (token.IsFailure)
		{
			return Error.Unauthorized("Token not found.");
		}

		Result<TokenValidationResult> validToken = await tokensManager.GetValidToken(token.Value.RawToken);
		if (validToken.IsFailure)
		{
			await accessTokens.UpdateTokenExpired(command.Token, ct);
			return Error.Unauthorized("Invalid token.");
		}

		return Unit.Value;
	}
}
