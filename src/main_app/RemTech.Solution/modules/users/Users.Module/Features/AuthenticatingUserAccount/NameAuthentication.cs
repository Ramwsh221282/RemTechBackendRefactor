using System.Data.Common;
using Npgsql;

namespace Users.Module.Features.AuthenticatingUserAccount;

internal sealed class NameAuthentication(string name, string password) : IUserAuthentication
{
    private const string FetchUserSql = """
        SELECT u.id as user_id, 
               u.name as user_name, 
               u.password as user_password, 
               u.email as user_email, 
               u.email_confirmed as user_email_confirmed, 
               r.name as role_name
        FROM users_module.users u
        LEFT JOIN users_module.user_roles ur ON u.id = ur.user_id
        LEFT JOIN users_module.roles r ON r.id = ur.role_id
        WHERE u.name = @name;
        """;

    public async Task<AuthenticatedUser> Authenticate(
        NpgsqlDataSource dataSource,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = FetchUserSql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", name));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        return !await reader.ReadAsync(ct)
            ? throw new AuthenticationUserNameNotFoundException(name)
            : new UserAuthenticationProcess(reader).ProcessAuthentication(password);
    }
}
