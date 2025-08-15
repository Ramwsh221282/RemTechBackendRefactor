using Cleaners.Module.Domain;
using Npgsql;

namespace Cleaners.Module.Database;

internal sealed class CleanerItemsByTresholdVeil : ICleanerVeil
{
    private const string Sql = """
        SELECT id, domain, source_url
        FROM contained_items.items
        WHERE is_deleted = FALSE
        AND created_at <= (@as_of::timestamp::date - @threshold);
        """;

    private Guid _id = Guid.Empty;
    private int _cleanedAmount;
    private DateTime _lastRun;
    private DateTime _nextRun;
    private int _waitDays;
    private string _state = string.Empty;
    private int _hours;
    private int _minutes;
    private int _seconds;
    private int _itemsDateDayThreshold;

    public void Accept(
        Guid id,
        int cleanedAmount,
        DateTime lastRun,
        DateTime nextRun,
        int waitDays,
        string state,
        int hours,
        int minutes,
        int seconds,
        int itemsDateDayThreshold
    )
    {
        _id = id;
        _cleanedAmount = cleanedAmount;
        _lastRun = lastRun;
        _nextRun = nextRun;
        _waitDays = waitDays;
        _state = state;
        _hours = hours;
        _minutes = minutes;
        _seconds = seconds;
        _itemsDateDayThreshold = itemsDateDayThreshold;
    }

    public ICleaner Behave()
    {
        return Cleaner.Create(
            _id,
            _cleanedAmount,
            _lastRun,
            _nextRun,
            _waitDays,
            _state,
            _hours,
            _minutes,
            _seconds,
            _itemsDateDayThreshold
        );
    }

    public Task<ICleaner> BehaveAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Behave());
    }

    public void FillCommand(NpgsqlCommand command)
    {
        command.CommandText = Sql;
        command.Parameters.Add(new NpgsqlParameter<DateTime>("@as_of", DateTime.UtcNow));
        command.Parameters.Add(new NpgsqlParameter<int>("@threshold", _itemsDateDayThreshold));
    }
}
