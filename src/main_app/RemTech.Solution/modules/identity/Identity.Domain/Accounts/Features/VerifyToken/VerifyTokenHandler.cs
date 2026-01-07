using Identity.Domain.Contracts.Jwt;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Extensions;
using Identity.Domain.Tokens;
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
        if (validToken.IsFailure) return Error.Unauthorized("Invalid token.");

        Result<AccessToken> token = await accessTokens.Get(validToken.Value.TokenId, ct);
        if (token.IsFailure) return Error.Unauthorized("Token not found.");

        return Unit.Value;
    }
}