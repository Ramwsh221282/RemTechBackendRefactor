using System.Data;
using System.Data.Common;
using Npgsql;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Postgres;
using Users.Module.CommonAbstractions;
using Users.Module.Features.CreatingNewAccount.Exceptions;

namespace Users.Module.Features.AddUserByAdmin;

internal sealed class AddUserByAdminCommandHandler(PostgresDatabase dataSource, StringHash hash)
    : ICommandHandler<AddUserByAdminCommand, Status<AddUserByAdminResult>>
{
    private const string InsertUserSql = """
        INSERT INTO users_module.users
        (id, name, password, email, email_confirmed)
        VALUES(@id, @name, @password, @email, @email_confirmed)
        """;

    private const string FetchRoleSql =
        "SELECT r.id FROM users_module.roles r WHERE r.name = @name";

    private const string AttachUserRoleSql =
        "INSERT INTO users_module.user_roles(user_id, role_id) VALUES(@user_id, @role_id)";

    public async Task<Status<AddUserByAdminResult>> Handle(
        AddUserByAdminCommand command,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.DataSource.OpenConnectionAsync(
            ct
        );
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(ct);

        try
        {
            (Guid userId, string password) data = await InsertUser(command, connection, ct);
            Status<Guid> role = await GetRoleId(command, connection, ct);
            if (role.IsFailure)
                return role.Error;

            await AttachUserRole(data.userId, role, connection, ct);
            await transaction.CommitAsync(ct);
            return new AddUserByAdminResult(
                data.userId,
                data.password,
                command.Name,
                command.Email,
                command.Role
            );
        }
        catch (NpgsqlException ex)
        {
            await transaction.RollbackAsync(ct);
            if (ex.Message.Contains("name"))
                throw new DuplicateNameException();
            if (ex.Message.Contains("email"))
                throw new EmailDuplicateException();
            throw;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    private async Task AttachUserRole(
        Guid userId,
        Guid roleId,
        NpgsqlConnection connection,
        CancellationToken ct
    )
    {
        await using NpgsqlCommand attachCommand = connection.CreateCommand();
        attachCommand.CommandText = AttachUserRoleSql;
        attachCommand.Parameters.Add(new NpgsqlParameter<Guid>("@user_id", userId));
        attachCommand.Parameters.Add(new NpgsqlParameter<Guid>("@role_id", roleId));
        await attachCommand.ExecuteNonQueryAsync(ct);
    }

    private async Task<(Guid, string)> InsertUser(
        AddUserByAdminCommand command,
        NpgsqlConnection connection,
        CancellationToken ct
    )
    {
        await using NpgsqlCommand insertCommand = connection.CreateCommand();
        Guid userId = Guid.NewGuid();
        string password = Guid.NewGuid().ToString();
        insertCommand.CommandText = InsertUserSql;
        insertCommand.Parameters.Add(new NpgsqlParameter<Guid>("@id", userId));
        insertCommand.Parameters.Add(new NpgsqlParameter<string>("@name", command.Name));
        insertCommand.Parameters.Add(new NpgsqlParameter<string>("@email", command.Email));
        string hashedPassword = hash.Hash(password);
        insertCommand.Parameters.Add(new NpgsqlParameter<string>("@password", hashedPassword));
        insertCommand.Parameters.Add(new NpgsqlParameter<bool>("@email_confirmed", false));
        await insertCommand.ExecuteNonQueryAsync(ct);
        return (userId, password);
    }

    private async Task<Status<Guid>> GetRoleId(
        AddUserByAdminCommand command,
        NpgsqlConnection connection,
        CancellationToken ct
    )
    {
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = FetchRoleSql;
        sqlCommand.Parameters.Add(new NpgsqlParameter<string>("@name", command.Role));
        await using DbDataReader reader = await sqlCommand.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            return Error.NotFound($"Роль не найдена: {command.Role}");
        Guid id = reader.GetGuid(0);
        return id;
    }
}
