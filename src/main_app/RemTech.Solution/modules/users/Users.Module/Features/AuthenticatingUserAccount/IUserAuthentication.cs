using Npgsql;

namespace Users.Module.Features.AuthenticatingUserAccount;

internal interface IUserAuthentication
{
    Task<AuthenticatedUser> Authenticate(
        NpgsqlDataSource dataSource,
        CancellationToken ct = default
    );
}
