using Identity.Domain.Accounts.Features.Authenticate;
using Identity.Domain.Accounts.Features.Refresh;
using Identity.Domain.Contracts.Persistence;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Infrastructure.Accounts.Commands.Refresh;

public sealed class RefreshTokenInvalidator(IAccessTokensRepository repository) 
    : ICacheInvalidator<RefreshTokenCommand, AuthenticationResult>
{
    public async Task InvalidateCache(
        RefreshTokenCommand command, 
        AuthenticationResult result,
        CancellationToken ct = default) =>
        await repository.Remove(result.AccessToken, ct);
}