using System.Data;
using System.Data.Common;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Infrastructure.Database;
using StackExchange.Redis;
using Timezones.Core.Contracts;
using Timezones.Core.Models;
using TimeZones.Infrastructure;

namespace TimeZones.Infrastructure.Persistence;

internal sealed class CachedRegionDateTimesRepository : IRegionDateTimesRepository
{
    public CachedRegionDateTimesRepository(IOptions<CachingOptions> options, NpgSqlSession session)
    {
        _options = options;
        _session = session;
    }

    private readonly NpgSqlSession _session;
    private const string TIMEZONES_ARRAY_KEY = "REGION_DATE_TIMES_ARRAY";
    private readonly IOptions<CachingOptions> _options;
    private static readonly Expiration _expiration = new(TimeSpan.FromMinutes(5));
    private ConnectionMultiplexer? _multiplexer;
    private IDatabase? _database;

    public async Task Refresh(IReadOnlyList<RegionLocalDateTime> dateTimes)
    {
        string json = dateTimes.ToJson();
        IDatabase database = await Database();
        await database.StringSetAsync(TIMEZONES_ARRAY_KEY, json, _expiration);
    }

    public async Task Save(RegionLocalDateTime dateTime, CancellationToken ct = default)
    {
        RegionLocalDateTime? existing = await Get(ct);
        if (existing is not null)
        {
            await Update(existing, dateTime, ct);
            return;
        }        

        await Insert(dateTime, ct);
    }

    private async Task Update(
        RegionLocalDateTime existing,
        RegionLocalDateTime @new,
        CancellationToken ct = default)
    {
        const string query =
                """
                UPDATE timezones_module.selected_timezone                
                SET region_name = @NewRegionName,                
                    offset_seconds = @OffsetSeconds,
                    timestamp_seconds = @TimestampSeconds,
                    year = @Year,
                    month = @Month,
                    day = @Day,
                    hour = @Hour,
                    minute = @Minute,
                    second = @Second
                WHERE region_name = @RegionName;
                """;

        PlainDateTimeInfo plainDate = @new.TimeZoneRecord.ToPlainDateTimeInfo();
        CommandDefinition command = new(query, new
        {
            NewRegionName = @new.TimeZoneRecord.ZoneName,
            OffsetSeconds = (long)@new.TimeZoneRecord.GmtOffset,
            TimestampSeconds = (long)@new.TimeZoneRecord.Timestamp,
            plainDate.Year,
            plainDate.Month,
            plainDate.Day,
            plainDate.Hour,
            Minute = plainDate.Minutes,
            Second = plainDate.Seconds,
            RegionName = existing.TimeZoneRecord.ZoneName
        }, transaction: _session.Transaction, cancellationToken: ct);

        NpgsqlConnection connection = await _session.GetConnection(ct);
        await connection.ExecuteAsync(command);
    }

    private async Task<RegionLocalDateTime?> Get(CancellationToken ct = default)
    {
        const string query =
            """ 
            SELECT
            region_name as region_name,
            offset_seconds as offset_seconds,
            timestamp_seconds as timestamp_seconds        
            FROM timezones_module.selected_timezone            
            LIMIT 1
            FOR UPDATE;
            """;

        CommandDefinition command = new(query, cancellationToken: ct, transaction: _session.Transaction);
        NpgsqlConnection connection = await _session.GetConnection(ct);
        await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
        if (!await reader.ReadAsync(ct))
        {
            return null;
        }

        return RegionLocalDateTime.FromDbReader(reader);
    }

    private async Task Insert(RegionLocalDateTime dateTime, CancellationToken ct = default)
    {
        const string query =
            """
            INSERT INTO timezones_module.selected_timezone
            (
                region_name,
                offset_seconds,
                timestamp_seconds,
                year,
                month,
                day,
                hour,
                minute,
                second
            )
            VALUES
            (
                @RegionName,
                @OffsetSeconds,
                @TimestampSeconds,
                @Year,
                @Month,
                @Day,
                @Hour,
                @Minute,
                @Second
            );
            """;

        PlainDateTimeInfo plainDate = dateTime.TimeZoneRecord.ToPlainDateTimeInfo();
        CommandDefinition command = new(query, new
        {
            RegionName = dateTime.TimeZoneRecord.ZoneName,
            OffsetSeconds = (long)dateTime.TimeZoneRecord.GmtOffset,
            TimestampSeconds = (long)dateTime.TimeZoneRecord.Timestamp,
            plainDate.Year,
            plainDate.Month,
            plainDate.Day,
            plainDate.Hour,
            Minute = plainDate.Minutes,
            Second = plainDate.Seconds
        }, transaction: _session.Transaction, cancellationToken: ct);

        NpgsqlConnection connection = await _session.GetConnection(ct);
        await connection.ExecuteAsync(command);
    }

    public async Task<IReadOnlyList<RegionLocalDateTime>> ProvideDateTimes()
    {
        IDatabase database = await Database();
        string? json = await database.StringGetAsync(TIMEZONES_ARRAY_KEY);
        return string.IsNullOrWhiteSpace(json) ? [] : RegionLocalDateTime.ToList(json);
    }

    private async Task<ConnectionMultiplexer> Multiplexer()
    {
        string connectionString = _options.Value.RedisConnectionString;
        return _multiplexer ??= await ConnectionMultiplexer.ConnectAsync(connectionString);
    }

    private async Task<IDatabase> Database()
    {
        ConnectionMultiplexer multiplexer = await Multiplexer();
        _database ??= multiplexer.GetDatabase();
        return _database;
    }
}

public static class RegionLocalDateTimePersistenceModule
{
    extension(RegionLocalDateTime)
    {
        public static RegionLocalDateTime FromDbReader(DbDataReader reader)
        {
            string name = reader.GetString("region_name");
            long offsetSeconds = reader.GetInt64("offset_seconds");
            long timestampSeconds = reader.GetInt64("timestamp_seconds");
            TimeZoneRecord timeZone = new()
            {
                ZoneName = name,
                GmtOffset = (ulong)offsetSeconds,
                Timestamp = (ulong)timestampSeconds
            };

            return timeZone.ToRegionLocalDateTime();
        }
    }
}