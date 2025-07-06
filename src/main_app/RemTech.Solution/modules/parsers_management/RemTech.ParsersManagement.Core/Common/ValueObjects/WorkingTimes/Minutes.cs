using RemTech.ParsersManagement.Core.Common.Primitives;

namespace RemTech.ParsersManagement.Core.Common.ValueObjects.WorkingTimes;

public sealed class Minutes
{
    private readonly PositiveInteger _minutes;

    public Minutes(PositiveInteger integer) => _minutes = integer;

    public Minutes(PositiveLong elapsed) =>
        _minutes = PositiveInteger.New(CalculateMinutesFromElapsedSeconds(elapsed.Read()));

    public PositiveInteger Read() => _minutes;

    private static int CalculateMinutesFromElapsedSeconds(long seconds) =>
        (int)((seconds % 3600) / 60);

    public static implicit operator int(Minutes minutes) => minutes._minutes;
}
