using System.Data.Common;
using Npgsql;
using RemTech.Core.Shared.Cqrs;
using Users.Module.Features.CreateEmailConfirmation;
using Users.Module.Models;

namespace Users.Module.Features.UpdateUserProfile;

internal sealed class UpdateUserRolePermissionWrapper(
    NpgsqlDataSource dataSource,
    ICommandHandler<UpdateUserProfileCommand, UpdateUserProfileResult> origin
) : ICommandHandler<UpdateUserProfileCommand, UpdateUserProfileResult>
{
    private const string Sql = """
        SELECT r.name FROM users_module.roles r
        LEFT JOIN users_module.user_roles ur ON r.id = ur.role_id
        LEFT JOIN users_module.users u ON u.id = ur.user_id 
        WHERE u.id = @id AND r.name = @name;
        """;

    public async Task<UpdateUserProfileResult> Handle(
        UpdateUserProfileCommand command,
        CancellationToken ct = default
    )
    {
        if (string.IsNullOrWhiteSpace(command.UpdateUserDetails.NewUserRole))
            return await origin.Handle(command, ct);
        UserJwtOutput output = command.Jwt.MakeOutput();
        Guid editorId = output.UserId;
        if (command.UpdateUserDetails.NewUserRole != "ROOT")
            return await origin.Handle(command, ct);
        if (!await IsEditorOfRole(editorId, "ROOT", ct))
            throw new OnlyRootUserCanPromoteRootException();
        return await origin.Handle(command, ct);
    }

    private async Task<bool> IsEditorOfRole(Guid editorId, string roleName, CancellationToken ct)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = Sql;
        command.Parameters.Add(new NpgsqlParameter<Guid>("@id", editorId));
        command.Parameters.Add(new NpgsqlParameter<string>("@name", roleName));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            throw new UserNotFoundException();
        string name = reader.GetString(0);
        return name == roleName;
    }
}
