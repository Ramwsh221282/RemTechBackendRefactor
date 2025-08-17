using Npgsql;
using Shared.Infrastructure.Module.Cqrs;
using Users.Module.Public;

namespace RemTech.ContainedItems.Module.Features.DropVehicles;

internal sealed class DropVehiclesHandler(
    PrivelegedAccessVerify access,
    NpgsqlDataSource dataSource
) : ICommandHandler<DropVehiclesCommand, int>
{
    private const string DeleteVehiclesSql = """
        DELETE FROM parsed_advertisements_module.parsed_vehicles pv
        USING contained_items.items i
        WHERE pv.id = i.id AND i.type = 'Техника';
        """;

    private const string DeleteContainedVehiclesSql =
        "DELETE FROM contained_items.items WHERE type = 'Техника'";

    public async Task<int> Handle(DropVehiclesCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.Email) && string.IsNullOrWhiteSpace(command.Name))
            throw new DropContainedItemsDeniedException();
        if (!string.IsNullOrWhiteSpace(command.Email) && string.IsNullOrWhiteSpace(command.Name))
            if (!await access.AuthenticateByEmail(command.Email, command.Password))
                throw new DropContainedItemsDeniedException();
        if (!string.IsNullOrWhiteSpace(command.Name) && string.IsNullOrWhiteSpace(command.Email))
            if (!await access.AuthenticateByUsername(command.Name, command.Password))
                throw new DropContainedItemsDeniedException();
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand npgsqlCommand = connection.CreateCommand();
        npgsqlCommand.CommandText = DeleteVehiclesSql;
        int affected = await npgsqlCommand.ExecuteNonQueryAsync(ct);
        npgsqlCommand.CommandText = DeleteContainedVehiclesSql;
        await npgsqlCommand.ExecuteNonQueryAsync(ct);
        return affected;
    }
}
