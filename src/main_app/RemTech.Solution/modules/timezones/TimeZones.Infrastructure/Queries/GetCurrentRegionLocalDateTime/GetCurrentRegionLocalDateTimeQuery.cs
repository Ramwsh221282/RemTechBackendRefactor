using System.Data.Common;
using Dapper;
using Npgsql;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;

namespace TimeZones.Infrastructure.Queries.GetCurrentRegionLocalDateTime;

public record GetCurrentRegionLocalDateTimeQuery : IQuery;

public sealed record CurrentRegionLocalDateTime(
    string RegionName,
    ulong TimestampSeconds,
    int OffsetSeconds,
    int Year,
    int Month,
    int Day,
    int Hour,
    int Minute,
    int Second
);

public sealed class GetCurrentRegionLocalDateTimeQueryHandler : IQueryHandler<GetCurrentRegionLocalDateTimeQuery, CurrentRegionLocalDateTime?>
{
    public GetCurrentRegionLocalDateTimeQueryHandler(NpgSqlSession session)
    {
        _session = session;
    }

    private readonly NpgSqlSession _session;

    public async Task<CurrentRegionLocalDateTime?> Handle(
        GetCurrentRegionLocalDateTimeQuery query, 
        CancellationToken ct = default)
    {
        const string sql =
        """
        SELECT  region_name as RegionName,
                timestamp_seconds as TimestampSeconds,
                offset_seconds as OffsetSeconds,
                year as Year,
                month as Month,
                day as Day,
                hour as Hour,
                minute as Minute,
                second as Second
        """;
        
        CommandDefinition command = new(sql, cancellationToken: ct, transaction: _session.Transaction);
        NpgsqlConnection connection = await _session.GetConnection(ct);
        await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
        if (!await reader.ReadAsync(ct))
        {
            return null;
        }

        return new CurrentRegionLocalDateTime(
            RegionName: reader.GetString(reader.GetOrdinal("RegionName")),
            TimestampSeconds: (ulong)reader.GetInt64(reader.GetOrdinal("TimestampSeconds")),
            OffsetSeconds: reader.GetInt32(reader.GetOrdinal("OffsetSeconds")),
            Year: reader.GetInt32(reader.GetOrdinal("Year")),
            Month: reader.GetInt32(reader.GetOrdinal("Month")),
            Day: reader.GetInt32(reader.GetOrdinal("Day")),
            Hour: reader.GetInt32(reader.GetOrdinal("Hour")),
            Minute: reader.GetInt32(reader.GetOrdinal("Minute")),
            Second: reader.GetInt32(reader.GetOrdinal("Second"))
        );
    }
}
