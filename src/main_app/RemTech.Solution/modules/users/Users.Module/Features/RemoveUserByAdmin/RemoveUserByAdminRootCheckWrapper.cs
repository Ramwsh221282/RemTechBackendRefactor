using System.Data.Common;
using Npgsql;
using RemTech.Core.Shared.Cqrs;
using Users.Module.Models;

namespace Users.Module.Features.RemoveUserByAdmin;

internal sealed class RemoveUserByAdminRootCheckWrapper(
    NpgsqlDataSource dataSource,
    ICommandHandler<RemoveUserByAdminCommand> origin
) : ICommandHandler<RemoveUserByAdminCommand>
{
    public async Task Handle(RemoveUserByAdminCommand command, CancellationToken ct = default)
    {
        UserJwtOutput output = command.Jwt.MakeOutput();
        if (output.Role == "USER")
            throw new ForbiddenOperationException();
        if (output.Role == "ADMIN")
        {
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            await using NpgsqlCommand npgCommand = connection.CreateCommand();
            npgCommand.CommandText = """
                SELECT r.name FROM users_module.roles r
                LEFT JOIN users_module.user_roles ur ON r.id = ur.role_id
                LEFT JOIN users_module.users u ON u.id = ur.user_id
                WHERE u.id = @id
                """;
            await using DbDataReader reader = await npgCommand.ExecuteReaderAsync(ct);
            if (!await reader.ReadAsync(ct))
                throw new UserNotFoundException();
            string role = reader.GetString(0);
            if (role is "ROOT" or "ADMIN")
                throw new ForbiddenOperationException();
        }
        await origin.Handle(command, ct);
    }
}
