using Npgsql;
using RemTech.Core.Shared.Cqrs;

namespace RemTech.ContainedItems.Module.Features.AddContainedItem;

internal sealed class AddContainedItemHandler(Serilog.ILogger logger, NpgsqlDataSource dataSource)
    : ICommandHandler<AddContainedItemCommand, int>
{
    private const string Sql = """
        INSERT INTO contained_items.items
        (id, type, domain, created_at, is_deleted, source_url)
        VALUES
        (@id, @type, @domain, @created_at, @is_deleted, @source_url)
        """;

    private const string IdParam = "@id";
    private const string TypeParam = "@type";
    private const string DomainParam = "@domain";
    private const string CreatedAtParam = "@created_at";
    private const string IsDeletedParam = "@is_deleted";
    private const string SourceUrlParam = "@source_url";

    public async Task<int> Handle(AddContainedItemCommand command, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        sqlCommand.Parameters.Add(new NpgsqlParameter<string>(IdParam, command.Item.Id));
        sqlCommand.Parameters.Add(new NpgsqlParameter<string>(TypeParam, command.Item.Type));
        sqlCommand.Parameters.Add(new NpgsqlParameter<string>(DomainParam, command.Item.Domain));
        sqlCommand.Parameters.Add(
            new NpgsqlParameter<DateTime>(CreatedAtParam, command.Item.CreatedAt)
        );
        sqlCommand.Parameters.Add(
            new NpgsqlParameter<bool>(IsDeletedParam, command.Item.IsDeleted)
        );
        sqlCommand.Parameters.Add(
            new NpgsqlParameter<string>(SourceUrlParam, command.Item.SourceUrl)
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
