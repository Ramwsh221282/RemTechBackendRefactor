using Cleaners.Adapter.Storage.Common;
using Cleaners.Adapter.Storage.DataModels;
using Cleaners.Domain.Cleaners.Aggregate;
using Cleaners.Domain.Cleaners.Ports;
using Cleaners.Domain.Cleaners.Ports.Storage;
using Dapper;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Postgres;

namespace Cleaners.Adapter.Storage.Storages;

public sealed class CleanersStorage(PostgresDatabase database) : ICleanersStorage
{
    public async Task<Status<Cleaner>> Get(CancellationToken ct = default)
    {
        string sql = CreateSql(limitClause: "LIMIT 1");
        var command = new CommandDefinition(sql, cancellationToken: ct);

        using var connection = await database.ProvideConnection(ct);
        var cleaner = await connection.QueryFirstOrDefaultAsync<CleanerDataModel>(command);

        return cleaner is null
            ? Error.NotFound("Чистильщик не найден.")
            : cleaner.ConvertToDomainModel();
    }

    public async Task<Status<Cleaner>> Get(Guid id, CancellationToken ct = default)
    {
        string sql = CreateSql(whereClauses: ["id = @id"], limitClause: "LIMIT 1");
        var command = new CommandDefinition(sql, new { id }, cancellationToken: ct);

        using var connection = await database.ProvideConnection(ct);
        var cleaner = await connection.QueryFirstOrDefaultAsync<CleanerDataModel>(command);

        return cleaner is null
            ? Error.NotFound("Чистильщик не найден.")
            : cleaner.ConvertToDomainModel();
    }

    private string CreateSql(List<string>? whereClauses = null, string? limitClause = null)
    {
        string whereClause =
            whereClauses == null ? string.Empty
            : whereClauses.Count == 0 ? string.Empty
            : " WHERE " + string.Join(" AND ", whereClauses);

        string limit = limitClause == null ? string.Empty : limitClause;

        string sql = $"""
            SELECT
            id,
            cleaned_amount,
            last_run,
            next_run,
            wait_days,
            state,
            hours,
            minutes,
            seconds,
            items_date_day_threshold
            FROM cleaners_module.cleaners
            {whereClause}
            {limit}
            """;

        return sql;
    }
}
