using Npgsql;

namespace Users.Module.Models.Features.AuthenticatingUserAccount;

internal interface IUserAuthentication
{
    Task<AuthenticatedUser> Authenticate(
        NpgsqlDataSource dataSource,
        CancellationToken ct = default
    );
}
