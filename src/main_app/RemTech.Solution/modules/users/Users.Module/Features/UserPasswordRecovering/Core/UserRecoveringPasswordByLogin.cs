using Npgsql;
using Users.Module.Features.UserPasswordRecovering.Exceptions;

namespace Users.Module.Features.UserPasswordRecovering.Core;

internal sealed class UserRecoveringPasswordByLogin : IUserRecoveringPassword
{
    private readonly string _login;

    private UserRecoveringPasswordByLogin(string login) => _login = login;

    public static UserRecoveringPasswordByLogin Create(string login)
    {
        return string.IsNullOrEmpty(login)
            ? throw new UserRecoveringPasswordByLoginEmptyException()
            : new UserRecoveringPasswordByLogin(login);
    }

    public void AddTo(NpgsqlCommand command)
    {
        string sql = """
            SELECT
            users.id as id,
            users.email as email,
            users.email_confirmed as email_confirmed
            FROM users_module.users as users
            WHERE users.name = @name;
            """;
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", _login));
    }
}
