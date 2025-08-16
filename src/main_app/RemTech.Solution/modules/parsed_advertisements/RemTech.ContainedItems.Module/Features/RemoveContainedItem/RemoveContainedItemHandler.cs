using Npgsql;
using Shared.Infrastructure.Module.Cqrs;

namespace RemTech.ContainedItems.Module.Features.RemoveContainedItem;

internal sealed class RemoveContainedItemHandler(
    Serilog.ILogger logger,
    NpgsqlDataSource dataSource
) : ICommandHandler<RemoveContainedItemCommand>
{
    private const string Sql = "UPDATE contained_items.items SET is_deleted = TRUE WHERE id = @id";

    public async Task Handle(RemoveContainedItemCommand command, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        command.Message.FillCommand(sqlCommand);
        await sqlCommand.ExecuteNonQueryAsync(ct);
        logger.Information("Item for removing has been added.");
    }
}
