using Npgsql;
using Shared.Infrastructure.Module.Cqrs;

namespace RemTech.ContainedItems.Module.Features.AddContainedItem;

internal sealed class AddContainedItemHandler(Serilog.ILogger logger, NpgsqlDataSource dataSource)
    : ICommandHandler<AddContainedItemCommand, int>
{
    public async Task<int> Handle(AddContainedItemCommand command, CancellationToken ct = default)
    {
        string sql = string.Intern(
            """
            INSERT INTO contained_items.items
            (id, type, domain, created_at, is_deleted, source_url)
            VALUES
            (@id, @type, @domain, @created_at, @is_deleted, @source_url)
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = sql;
        sqlCommand.Parameters.Add(new NpgsqlParameter<string>("@id", command.Item.Id));
        sqlCommand.Parameters.Add(new NpgsqlParameter<string>("@type", command.Item.Type));
        sqlCommand.Parameters.Add(new NpgsqlParameter<string>("@domain", command.Item.Domain));
        sqlCommand.Parameters.Add(
            new NpgsqlParameter<DateTime>("@created_at", command.Item.CreatedAt)
        );
        sqlCommand.Parameters.Add(new NpgsqlParameter<bool>("@is_deleted", command.Item.IsDeleted));
        sqlCommand.Parameters.Add(
            new NpgsqlParameter<string>("@source_url", command.Item.SourceUrl)
        );
        int affected = await sqlCommand.ExecuteNonQueryAsync(ct);
        if (affected == 0)
        {
            logger.Warning(
                "{Command} item was not added. Duplicate by source url or ID.",
                nameof(AddContainedItemCommand)
            );
        }
        else
        {
            logger.Information(
                "{Command} contained item has been added.",
                nameof(AddContainedItemCommand)
            );
        }

        return affected;
    }
}
