using RemTech.ParsersManagement.Core.Common.Primitives;

namespace RemTech.ParsersManagement.Core.Common.ValueObjects.WorkingTimes;

public sealed class WorkingTime
{
    private readonly PositiveLong _totalElapsed;
    private readonly Hour _hours;
    private readonly Minutes _minutes;
    private readonly Seconds _seconds;

    public WorkingTime(PositiveLong totalElapsed, Hour hours, Minutes minutes, Seconds seconds)
    {
        _totalElapsed = totalElapsed;
        _hours = hours;
        _minutes = minutes;
        _seconds = seconds;
    }

    public WorkingTime()
    {
        _totalElapsed = PositiveLong.New();
        int zeroInteger = PositiveInteger.New();
        _hours = new Hour(PositiveInteger.New(zeroInteger));
        _minutes = new Minutes(PositiveInteger.New(zeroInteger));
        _seconds = new Seconds(PositiveInteger.New(zeroInteger));
    }

    public WorkingTime(PositiveLong elapsed)
    {
        _totalElapsed = elapsed;
        _hours = new Hour(elapsed);
        _minutes = new Minutes(elapsed);
        _seconds = new Seconds(elapsed);
    }

    public PositiveLong Total() => _totalElapsed;

    public Hour Hours() => _hours;

    public Minutes Minutes() => _minutes;

    public Seconds Seconds() => _seconds;
}
