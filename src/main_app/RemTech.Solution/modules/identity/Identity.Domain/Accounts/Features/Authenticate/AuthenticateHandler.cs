using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Cryptography;
using Identity.Domain.Contracts.Jwt;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace Identity.Domain.Accounts.Features.Authenticate;

[TransactionalHandler]
public sealed class AuthenticateHandler(
    IAccountsRepository accounts,
    IJwtTokenManager tokenManager,
    IPasswordHasher hasher,
    IAccessTokensRepository accessTokens,
    IRefreshTokensRepository refreshTokens
) : ICommandHandler<AuthenticateCommand, AuthenticationResult>
{
    public async Task<Result<AuthenticationResult>> Execute(AuthenticateCommand command, CancellationToken ct = default)
    {
        Result<Account> account = await GetRequiredAccount(command, ct);
        if (account.IsFailure)
            return account.Error;

        Result<Unit> verification = account.Value.VerifyPassword(command.Password, hasher);
        if (verification.IsFailure)
            return verification.Error;

        AccessToken tokenData = tokenManager.GenerateToken(account.Value);
        await accessTokens.Add(tokenData, ct);

        RefreshToken refreshToken = tokenManager.GenerateRefreshToken(account.Value.Id.Value);
        await refreshTokens.Add(refreshToken, ct);
        return CreateAuthenticationResult(tokenData.RawToken, refreshToken);
    }

    private static AuthenticationResult CreateAuthenticationResult(string token, RefreshToken refreshToken) =>
        new(AccessToken: token, RefreshToken: refreshToken.TokenValue);

    private async Task<Result<Account>> GetRequiredAccount(AuthenticateCommand command, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(command.Login) && string.IsNullOrEmpty(command.Email))
            return Error.Validation("Логин или email не указаны.");

        AccountSpecification spec = new AccountSpecification().WithLock();
        if (!string.IsNullOrEmpty(command.Login))
            spec = spec.WithLogin(command.Login);
        if (!string.IsNullOrEmpty(command.Email))
            spec = spec.WithEmail(command.Email);

        return await accounts.Find(spec, ct);
    }
}
