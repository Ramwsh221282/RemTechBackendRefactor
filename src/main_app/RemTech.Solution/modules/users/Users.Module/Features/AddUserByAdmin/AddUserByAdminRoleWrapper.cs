using System.Data.Common;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;
using Users.Module.Features.CreateEmailConfirmation;

namespace Users.Module.Features.AddUserByAdmin;

internal sealed class AddUserByAdminRoleWrapper(
    NpgsqlDataSource dataSource,
    ICommandHandler<AddUserByAdminCommand, AddUserByAdminResult> origin
) : ICommandHandler<AddUserByAdminCommand, AddUserByAdminResult>
{
    private const string Sql = """
        SELECT r.name as role_name
        FROM users_module.users u
        LEFT JOIN users_module.user_roles ur ON ur.user_id = u.id
        LEFT JOIN users_module.roles r ON r.id = ur.role_id
        WHERE u.id = @id;
        """;

    public async Task<AddUserByAdminResult> Handle(
        AddUserByAdminCommand command,
        CancellationToken ct = default
    )
    {
        Guid userId = command.AdditorId;
        string role = await GetRole(userId, ct);
        if (command.Role == "ROOT" && role != "ROOT")
            throw new RootUserCanBeAddedOnlyByRootException();
        return await origin.Handle(command, ct);
    }

    private async Task<string> GetRole(Guid id, CancellationToken ct)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand insertCommand = connection.CreateCommand();
        insertCommand.CommandText = Sql;
        insertCommand.Parameters.Add(new NpgsqlParameter<Guid>("@id", id));
        await using DbDataReader reader = await insertCommand.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            throw new UserNotFoundException();
        string role = reader.GetString(0);
        return role;
    }
}
