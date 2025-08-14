using Cleaners.Module.Domain;
using Npgsql;

namespace Cleaners.Module.Database;

internal sealed class NpgSqlSavingCleanerVeil(NpgsqlConnection connection) : ICleanerVeil
{
    private const string SaveSql = """
        INSERT INTO cleaners_module.cleaners
        (id, cleaned_amount, last_run, next_run, wait_days, state, hours, minutes, seconds)
        VALUES(@id, @cleanedAmount, @lastRun, @nextRun, @waitDays, @state, @hours, @minutes, @seconds)
        ON CONFLICT(id) 
        DO UPDATE SET
               cleaned_amount = @cleanedAmount,
               last_run = @lastRun,
               next_run = @nextRun,
               wait_days = @waitDays,
               hours = @hours,
               minutes = @minutes,
               seconds = @seconds,
               state = @state;
        """;

    private Guid _id = Guid.NewGuid();
    private int _cleanedAmount;
    private DateTime _lastRun = DateTime.MinValue;
    private DateTime _nextRun = DateTime.MinValue;
    private int _waitDays;
    private string _state = "NONE";
    private int _hours;
    private int _minutes;
    private int _seconds;

    public void Accept(
        Guid id,
        int cleanedAmount,
        DateTime lastRun,
        DateTime nextRun,
        int waitDays,
        string state,
        int hours,
        int minutes,
        int seconds
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
    }

    public ICleaner Behave()
    {
        using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = SaveSql;
        FillCommandParameters(command);
        command.ExecuteNonQuery();
        return Cleaner.Create(
            _id,
            _cleanedAmount,
            _lastRun,
            _nextRun,
            _waitDays,
            _state,
            _hours,
            _minutes,
            _seconds
        );
    }

    public async Task<ICleaner> BehaveAsync(CancellationToken ct = default)
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = SaveSql;
        FillCommandParameters(command);
        await command.ExecuteNonQueryAsync(ct);
        return Cleaner.Create(
            _id,
            _cleanedAmount,
            _lastRun,
            _nextRun,
            _waitDays,
            _state,
            _hours,
            _minutes,
            _seconds
        );
    }

    private void FillCommandParameters(NpgsqlCommand command)
    {
        command.Parameters.Add(new NpgsqlParameter("@id", _id));
        command.Parameters.Add(new NpgsqlParameter("@cleanedAmount", _cleanedAmount));
        command.Parameters.Add(new NpgsqlParameter("@lastRun", _lastRun));
        command.Parameters.Add(new NpgsqlParameter("@nextRun", _nextRun));
        command.Parameters.Add(new NpgsqlParameter("@waitDays", _waitDays));
        command.Parameters.Add(new NpgsqlParameter("@state", _state));
        command.Parameters.Add(new NpgsqlParameter("@hours", _hours));
        command.Parameters.Add(new NpgsqlParameter("@minutes", _minutes));
        command.Parameters.Add(new NpgsqlParameter("@seconds", _seconds));
    }
}
