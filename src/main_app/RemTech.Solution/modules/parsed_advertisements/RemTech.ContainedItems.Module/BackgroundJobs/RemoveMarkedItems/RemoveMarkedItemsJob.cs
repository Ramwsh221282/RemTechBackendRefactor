using Npgsql;
using Quartz;

namespace RemTech.ContainedItems.Module.BackgroundJobs.RemoveMarkedItems;

public sealed class RemoveMarkedItemsJob(NpgsqlDataSource dataSource, Serilog.ILogger logger) : IJob
{
    private const string DeleteVehiclesSql = """
        DELETE FROM parsed_advertisements_module.parsed_vehicles pv
        USING contained_items.items i
        WHERE pv.id = i.id AND i.is_deleted = TRUE;
        """;

    private const string DeleteSparesSql = """
        DELETE FROM spares_module.spares s
        USING contained_items.items i
        WHERE s.id = i.id AND i.is_deleted = TRUE;
        """;

    public async Task Execute(IJobExecutionContext context)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
        try
        {
            await using NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = DeleteVehiclesSql;
            int vehicles = await command.ExecuteNonQueryAsync();
            command.CommandText = DeleteSparesSql;
            int spares = await command.ExecuteNonQueryAsync();
            await transaction.CommitAsync();
            logger.Information("Removed vehicles: {Veh}. Removed spares: {Sp}", vehicles, spares);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.Fatal("{Entrance}. {Ex}.", nameof(RemoveMarkedItemsJob), ex.Message);
        }
    }
}
