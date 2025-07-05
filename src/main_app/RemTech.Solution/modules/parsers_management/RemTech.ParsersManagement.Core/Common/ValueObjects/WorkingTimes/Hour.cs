using RemTech.ParsersManagement.Core.Common.Primitives;

namespace RemTech.ParsersManagement.Core.Common.ValueObjects.WorkingTimes;

public sealed class Hour
{
    private readonly PositiveInteger _hours;

    public Hour(PositiveInteger hours) => _hours = hours;

    public Hour(PositiveLong elapsed) =>
        _hours = PositiveInteger.New(CalculateHoursFromElapsedSeconds(elapsed.Read()));

    public PositiveInteger Read() => _hours;

    private static int CalculateHoursFromElapsedSeconds(long seconds) => (int)(seconds / 3600);
}
