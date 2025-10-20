using System.Data.Common;
using Npgsql;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Postgres;

namespace Users.Module.Features.AddUserByAdmin;

internal sealed class AddUserByAdminRoleWrapper(
    PostgresDatabase dataSource,
    ICommandHandler<AddUserByAdminCommand, Status<AddUserByAdminResult>> origin
) : ICommandHandler<AddUserByAdminCommand, Status<AddUserByAdminResult>>
{
    private const string Sql = """
        SELECT r.name as role_name
        FROM users_module.users u
        LEFT JOIN users_module.user_roles ur ON ur.user_id = u.id
        LEFT JOIN users_module.roles r ON r.id = ur.role_id
        WHERE u.id = @id;
        """;

    public async Task<Status<AddUserByAdminResult>> Handle(
        AddUserByAdminCommand command,
        CancellationToken ct = default
    )
    {
        Guid userId = command.AdditorId;
        Status<string> role = await GetRole(userId, command.Role, ct);
        return role.IsFailure ? role.Error : await origin.Handle(command, ct);
    }

    private async Task<Status<string>> GetRole(Guid id, string commandRole, CancellationToken ct)
    {
        await using NpgsqlConnection connection = await dataSource.DataSource.OpenConnectionAsync(
            ct
        );

        await using NpgsqlCommand insertCommand = connection.CreateCommand();
        insertCommand.CommandText = Sql;
        insertCommand.Parameters.Add(new NpgsqlParameter<Guid>("@id", id));
        await using DbDataReader reader = await insertCommand.ExecuteReaderAsync(ct);

        if (!await reader.ReadAsync(ct))
            return Error.NotFound("Пользователь не найден.");

        string role = reader.GetString(0);

        if (commandRole == "ROOT" && role != "ROOT")
            return Error.Conflict(
                "Корневой пользователь может быть добавлен только корневым пользователем."
            );

        return role;
    }
}
