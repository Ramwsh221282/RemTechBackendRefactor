using Npgsql;
using RemTech.ContainedItems.Module.Features.DropVehicles;
using Shared.Infrastructure.Module.Cqrs;
using Users.Module.Public;

namespace RemTech.ContainedItems.Module.Features.DropSpares;

internal sealed class DropSparesHandler(PrivelegedAccessVerify access, NpgsqlDataSource dataSource)
    : ICommandHandler<DropSparesCommand, int>
{
    private const string DeleteSparesSql = """
        DELETE FROM spares_module.spares s
        USING contained_items.items i
        WHERE s.id = i.id AND i.type = 'Запчасти';
        """;

    private const string DeleteContainedVehiclesSql =
        "DELETE FROM contained_items.items WHERE type = 'Запчасти'";

    public async Task<int> Handle(DropSparesCommand command, CancellationToken ct = default)
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
        npgsqlCommand.CommandText = DeleteSparesSql;
        int affected = await npgsqlCommand.ExecuteNonQueryAsync(ct);
        npgsqlCommand.CommandText = DeleteContainedVehiclesSql;
        await npgsqlCommand.ExecuteNonQueryAsync(ct);
        return affected;
    }
}
