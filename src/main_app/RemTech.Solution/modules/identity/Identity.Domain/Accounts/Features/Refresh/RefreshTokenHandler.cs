using Identity.Domain.Accounts.Features.Authenticate;
using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Jwt;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;

namespace Identity.Domain.Accounts.Features.Refresh;

[TransactionalHandler]
public sealed class RefreshTokenHandler(
    IAccessTokensRepository accessTokens, 
    IRefreshTokensRepository refreshTokens,
    IAccountsRepository accounts,
    IJwtTokenManager tokenManager) : ICommandHandler<RefreshTokenCommand, AuthenticationResult>
{
    public async Task<Result<AuthenticationResult>> Execute(
        RefreshTokenCommand command, 
        CancellationToken ct = default)
    {
        Result<RefreshToken> refreshToken = await refreshTokens.Get(command.RefreshToken, withLock: true, ct);
        if (refreshToken.IsFailure) 
            return Error.Unauthorized("Token not found.");
        // if (refreshToken.Value.IsExpired()) 
        //     return Error.Unauthorized("Invalid token."); // TODO decide how to handle expired token.
        
        Result<Account> account = await GetRequiredAccount(refreshToken.Value, ct);
        if (account.IsFailure) 
            return Error.Unauthorized("Account not found.");
        
        AccessToken newAccessToken = tokenManager.GenerateToken(account.Value);
        RefreshToken newRefreshToken = tokenManager.GenerateRefreshToken(account.Value.Id.Value);
        
        await accessTokens.Add(newAccessToken, ct);
        await refreshTokens.Update(newRefreshToken, ct);
        
        return new AuthenticationResult(newAccessToken.RawToken, newRefreshToken.TokenValue);
    }
    
    private async Task<Result<Account>> GetRequiredAccount(RefreshToken refreshToken, CancellationToken ct)
    {
        AccountSpecification spec = new AccountSpecification()
            .WithId(refreshToken.AccountId)
            .WithLock();
        return await accounts.Get(spec, ct);
    }
}