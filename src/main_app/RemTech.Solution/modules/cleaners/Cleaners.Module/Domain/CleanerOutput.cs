namespace Cleaners.Module.Domain;

internal sealed class CleanerOutput
{
    private readonly Guid _id;
    private readonly int _cleanedAmount;
    private readonly DateTime _lastRun;
    private readonly DateTime _nextRun;
    private readonly int _waitDays;
    private readonly string _state;
    private readonly int _hours;
    private readonly int _minutes;
    private readonly int _seconds;

    public CleanerOutput(
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

    public TCleanerVeil PrintTo<TCleanerVeil>(TCleanerVeil storage)
        where TCleanerVeil : ICleanerVeil
    {
        storage.Accept(
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
        return storage;
    }
}
