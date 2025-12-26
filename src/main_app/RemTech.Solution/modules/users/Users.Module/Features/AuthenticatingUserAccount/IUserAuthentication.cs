using Shared.Infrastructure.Module.Postgres;

namespace Users.Module.Features.AuthenticatingUserAccount;

internal interface IUserAuthentication
{
    Task<AuthenticatedUser> Authenticate(
        PostgresDatabase dataSource,
        CancellationToken ct = default
    );
}
