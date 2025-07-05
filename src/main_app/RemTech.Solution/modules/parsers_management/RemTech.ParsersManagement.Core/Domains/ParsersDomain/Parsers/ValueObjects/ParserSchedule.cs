using RemTech.ParsersManagement.Core.Common.Primitives;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

public sealed class ParserSchedule
{
    private readonly DateTime _lastRun;
    private readonly DateTime _nextRun;
    private readonly PositiveInteger _waitDays;
    private readonly PositiveInteger _maxWaitDays = PositiveInteger.New(7);
    private readonly PositiveInteger _minWaitDays = PositiveInteger.New(1);

    public ParserSchedule(DateTime lastRun, DateTime nextRun, PositiveInteger waitDays)
    {
        _lastRun = lastRun;
        _nextRun = nextRun;
        _waitDays = waitDays;
    }

    public ParserSchedule()
    {
        DateTime utcNow = DateTime.UtcNow;
        _lastRun = utcNow;
        _nextRun = utcNow;
        _waitDays = PositiveInteger.New(1);
    }

    public ParserSchedule Next()
    {
        DateTime utcNow = DateTime.UtcNow;
        DateTime nextRun = utcNow.AddDays(_waitDays.Read());
        return new ParserSchedule(utcNow, nextRun, _waitDays);
    }

    public ParserSchedule OtherWaitDays(PositiveInteger waitDays)
    {
        DateTime nextRun = _lastRun.AddDays(waitDays.Read());
        return new ParserSchedule(_lastRun, nextRun, waitDays);
    }

    public bool WaitDaysTooMuch(PositiveInteger waitDays) => waitDays.BiggerThan(_maxWaitDays);

    public bool WaitDaysTooLess(PositiveInteger waitDays) => waitDays.LesserThan(_minWaitDays);

    public DateTime LastRun() => _lastRun;

    public DateTime NextRun() => _nextRun;

    public PositiveInteger WaitDays() => _waitDays;
}
