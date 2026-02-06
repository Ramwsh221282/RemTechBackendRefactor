using Identity.Domain.Accounts.Features.Authenticate;
using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Jwt;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace Identity.Domain.Accounts.Features.Refresh;

/// <summary>
/// Обработчик команды для обновления токенов пользователя.
/// </summary>
/// <param name="accessTokens">Репозиторий токенов доступа.</param>
/// <param name="refreshTokens">Репозиторий токенов обновления.</param>
/// <param name="accounts">Репозиторий аккаунтов.</param>
/// <param name="tokenManager">Менеджер JWT токенов.</param>
[TransactionalHandler]
public sealed class RefreshTokenHandler(
	IAccessTokensRepository accessTokens,
	IRefreshTokensRepository refreshTokens,
	IAccountsRepository accounts,
	IJwtTokenManager tokenManager
) : ICommandHandler<RefreshTokenCommand, AuthenticationResult>
{
	/// <summary>
	/// Выполняет обновление токенов пользователя по команде.
	/// </summary>
	/// <param name="command">Команда обновления токенов пользователя.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды.</returns>
	public async Task<Result<AuthenticationResult>> Execute(RefreshTokenCommand command, CancellationToken ct = default)
	{
		Result<RefreshToken> refreshToken = await refreshTokens.Find(command.RefreshToken, withLock: true, ct);
		if (refreshToken.IsFailure)
		{
			return Error.Unauthorized("Token not found.");
		}

		if (refreshToken.Value.IsExpired(tokenManager))
		{
			await refreshTokens.Delete(refreshToken.Value, ct);
			return Error.Unauthorized("Token is expired.");
		}

		Result<Account> account = await GetRequiredAccount(refreshToken.Value, ct);
		if (account.IsFailure)
		{
			return Error.Unauthorized("Account not found.");
		}

		AccessToken newAccessToken = tokenManager.GenerateToken(account.Value);
		RefreshToken newRefreshToken = tokenManager.GenerateRefreshToken(account.Value.Id.Value);

		await accessTokens.Add(newAccessToken, ct);
		await refreshTokens.Update(newRefreshToken, ct);

		return new AuthenticationResult(newAccessToken.RawToken, newRefreshToken.TokenValue);
	}

	private Task<Result<Account>> GetRequiredAccount(RefreshToken refreshToken, CancellationToken ct)
	{
		AccountSpecification spec = new AccountSpecification().WithId(refreshToken.AccountId).WithLock();
		return accounts.Find(spec, ct);
	}
}
