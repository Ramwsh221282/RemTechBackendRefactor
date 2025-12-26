using Npgsql;
using Users.Module.CommonAbstractions;

namespace Users.Module.Features.UserPasswordRecovering.Core;

internal sealed class UserRecoveringPasswordByEmail : IUserRecoveringPassword
{
    private readonly string _email;

    private UserRecoveringPasswordByEmail(string email) => _email = email;

    public static UserRecoveringPasswordByEmail Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new UserRecoveringPasswordByEmailInvalidException();
        new EmailValidation().ValidateEmail(email);
        return new UserRecoveringPasswordByEmail(email);
    }

    public void AddTo(NpgsqlCommand command)
    {
        string sql = """
            SELECT
            users.id as id,
            users.email as email,
            users.email_confirmed as email_confirmed
            FROM users_module.users as users
            WHERE users.email = @email;
            """;
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@email", _email));
    }
}
