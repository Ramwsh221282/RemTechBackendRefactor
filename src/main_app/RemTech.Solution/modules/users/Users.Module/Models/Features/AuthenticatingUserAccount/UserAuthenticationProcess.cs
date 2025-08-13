using System.Data.Common;

namespace Users.Module.Models.Features.AuthenticatingUserAccount;

internal sealed class UserAuthenticationProcess(DbDataReader reader)
{
    public AuthenticatedUser ProcessAuthentication(string password)
    {
        string userPassword = reader.GetString(reader.GetOrdinal("user_password"));
        return !BCrypt.Net.BCrypt.Verify(password, userPassword)
            ? throw new AuthenticationPasswordFailedException()
            : CompleteUser(reader, userPassword);
    }

    private static AuthenticatedUser CompleteUser(DbDataReader reader, string userPassword)
    {
        Guid userId = reader.GetGuid(reader.GetOrdinal("user_id"));
        string userName = reader.GetString(reader.GetOrdinal("user_name"));
        string userEmail = reader.GetString(reader.GetOrdinal("user_email"));
        bool userEmailConfirmed = reader.GetBoolean(reader.GetOrdinal("user_email_confirmed"));
        string roleName = reader.GetString(reader.GetOrdinal("role_name"));
        return new AuthenticatedUser(
            userId,
            userName,
            userEmail,
            userPassword,
            userEmailConfirmed,
            roleName
        );
    }
}
