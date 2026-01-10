using Identity.Domain.Contracts.Jwt;
using Identity.Domain.Contracts.Persistence;
using Microsoft.IdentityModel.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.VerifyToken;

public sealed class VerifyTokenHandler(
    IAccessTokensRepository accessTokens,
    IJwtTokenManager tokensManager)
    : ICommandHandler<VerifyTokenCommand, Unit>
{
    public async Task<Result<Unit>> Execute(VerifyTokenCommand command, CancellationToken ct = default)
    {
        Result<TokenValidationResult> validToken = await tokensManager.GetValidToken(command.Token);
        if (validToken.IsFailure)
        {
            await accessTokens.UpdateTokenExpired(command.Token, ct);
            return Error.Unauthorized("Invalid token.");
        }
        
        return Unit.Value;
    }
}