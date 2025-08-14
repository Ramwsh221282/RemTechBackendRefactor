using Cleaners.Module.Domain;

namespace Cleaners.Module.Services;

internal sealed class LoggingCleanerVeil(Serilog.ILogger logger) : ICleanerVeil
{
    private Guid _id = Guid.NewGuid();
    private int _cleanedAmount = 0;
    private DateTime _lastRun = DateTime.MinValue;
    private DateTime _nextRun = DateTime.MinValue;
    private int _waitDays = 0;
    private string _state = "NONE";
    private int _hours = 0;
    private int _minutes = 0;
    private int _seconds = 0;

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
        Log();
        return ReturnBack();
    }

    public Task<ICleaner> BehaveAsync(CancellationToken ct = default) => Task.FromResult(Behave());

    private void Log() =>
        logger.Information(
            """
            CLEANER INFORMATION:
            ID: {ID}
            CLEANED AMOUNT: {CLEANED_AMOUNT}
            LAST RUN: {LAST_RUN}
            NEXT RUN: {NEXT_RUN}
            WAIT DAYS: {WAIT_DAYS}
            STATE: {STATE}
            WORKED TIME: {H} h. {M} m. {S} s.
            """,
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

    private ICleaner ReturnBack() =>
        Cleaner.Create(
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
