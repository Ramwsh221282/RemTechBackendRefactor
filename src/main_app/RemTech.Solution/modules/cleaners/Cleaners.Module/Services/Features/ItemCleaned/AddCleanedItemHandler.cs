using Cleaners.Module.Database;
using Cleaners.Module.Domain;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;

namespace Cleaners.Module.Services.Features.ItemCleaned;

internal sealed class AddCleanedItemHandler(NpgsqlConnection connection, Serilog.ILogger logger)
    : ICommandHandler<AddCleanedItem, ICleaner>
{
    public async Task<ICleaner> Handle(AddCleanedItem command, CancellationToken ct = default)
    {
        logger.Information("Adding cleaned item");
        ICleaners cleaners = new NpgSqlCleaners(connection);
        ICleaner cleaner = await cleaners.Single(ct);
        return cleaner.CleanItem();
    }
}
