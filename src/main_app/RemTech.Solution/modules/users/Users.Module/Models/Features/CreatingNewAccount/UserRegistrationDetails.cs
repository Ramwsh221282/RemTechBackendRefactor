using System.Data.Common;
using Npgsql;
using Users.Module.CommonAbstractions;
using Users.Module.Models.Features.CreatingNewAccount.Exceptions;

namespace Users.Module.Models.Features.CreatingNewAccount;

internal sealed class UserRegistrationDetails(UserRegistration registration)
{
    private string _roleName = string.Empty;

    public async Task<UserRegistrationDetails> SaveIn(
        NpgsqlDataSource dataSource,
        StringHash hash,
        string roleName,
        CancellationToken ct,
        bool isRoot = false
    )
    {
        registration.NameLengthSatisfied();
        registration.PasswordDifficultySatisfied();
        registration.EmailDifficultySatisfied();
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        registration.FillDetails(command, hash);
        try
        {
            (Guid, string) role = isRoot switch
            {
                false => await GetUserRole(connection, roleName, ct),
                true => await GetUserRole(connection, "ROOT", ct),
            };
            await using NpgsqlCommand userIdCommand = connection.CreateCommand();
            registration.FillDetails(userIdCommand, hash, isRoot);
            await command.ExecuteNonQueryAsync(ct);
            await SaveUserRole(userIdCommand, role.Item1, ct);
            await transaction.CommitAsync(ct);
            _roleName = role.Item2;
            return this;
        }
        catch (PostgresException ex)
        {
            await transaction.RollbackAsync(ct);
            if (ex.Message.Contains("users_email_key"))
                throw new EmailDuplicateException();
            if (ex.Message.Contains("users_name_key"))
                throw new NameDuplicateException();
            throw;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public UserRegistrationJwtDetails JwtDetails()
    {
        UserRegistrationJwtDetails jwtDetails = new();
        jwtDetails.AddUserRole(_roleName);
        registration.FillDetails(jwtDetails);
        return jwtDetails;
    }

    private async Task<(Guid, string)> GetUserRole(
        NpgsqlConnection connection,
        string roleName,
        CancellationToken ct = default
    )
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = "SELECT id, name FROM users_module.roles WHERE name = @name";
        command.Parameters.Add(new NpgsqlParameter<string>("@name", roleName));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new ApplicationException("Роль пользователя USER не создана");
        await reader.ReadAsync(ct);
        Guid id = reader.GetGuid(0);
        string name = reader.GetString(1);
        return (id, name);
    }

    private async Task SaveUserRole(
        NpgsqlCommand userId,
        Guid roleId,
        CancellationToken ct = default
    )
    {
        userId.CommandText =
            "INSERT INTO users_module.user_roles (user_id, role_id) VALUES (@user_id, @role_id)";
        userId.Parameters.Add(new NpgsqlParameter<Guid>("@role_id", roleId));
        await userId.ExecuteNonQueryAsync(ct);
    }
}
