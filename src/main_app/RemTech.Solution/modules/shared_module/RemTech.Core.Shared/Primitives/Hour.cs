namespace RemTech.Core.Shared.Primitives;

public readonly record struct Hour
{
    private readonly PositiveInteger _hours = new();

    public Hour(PositiveInteger hours) => _hours = hours;

    public Hour(PositiveLong elapsed) =>
        _hours = PositiveInteger.New(CalculateHoursFromElapsedSeconds(elapsed.Read()));

    public PositiveInteger Read() => _hours;

    private static int CalculateHoursFromElapsedSeconds(long seconds) => (int)(seconds / 3600);

    public static implicit operator int(Hour hour) => hour._hours;
}
