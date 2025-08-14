using Cleaners.Module.Domain;

namespace Cleaners.Module.Services;

internal sealed class LoggingCleanerVeil(Serilog.ILogger logger) : ICleanerVeil
{
    private Guid _id = Guid.NewGuid();
    private int _cleanedAmount;
    private DateTime _lastRun = DateTime.MinValue;
    private DateTime _nextRun = DateTime.MinValue;
    private int _waitDays;
    private string _state = "NONE";
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
            ITEMS DATE DAYS THRESHOLD: {THRESHOLD}
            """,
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
            _seconds,
            _itemsDateDayThreshold
        );
}
