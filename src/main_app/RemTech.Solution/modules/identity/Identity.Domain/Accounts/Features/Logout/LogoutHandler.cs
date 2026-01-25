using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace Identity.Domain.Accounts.Features.Logout;

[TransactionalHandler]
public sealed class LogoutHandler(IAccessTokensRepository accessTokens, IRefreshTokensRepository refreshTokens)
    : ICommandHandler<LogoutCommand, Unit>
{
    public async Task<Result<Unit>> Execute(LogoutCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.AccessToken))
            return Error.Validation("Access token is empty.");
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
            return Error.Validation("Refresh token is empty.");

        Result<AccessToken> accessToken = await accessTokens.Find(command.AccessToken, withLock: true, ct: ct);
        Result<RefreshToken> refreshToken = await refreshTokens.Find(command.RefreshToken, withLock: true, ct: ct);

        if (accessToken.IsSuccess)
            await accessTokens.Remove(accessToken.Value, ct);
        if (refreshToken.IsSuccess)
            await refreshTokens.Delete(refreshToken.Value, ct);
        return Unit.Value;
    }
}
