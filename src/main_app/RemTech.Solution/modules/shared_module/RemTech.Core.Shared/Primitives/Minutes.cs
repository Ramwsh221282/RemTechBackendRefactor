namespace RemTech.Core.Shared.Primitives;

public readonly record struct Minutes
{
    private readonly PositiveInteger _minutes = new();

    public Minutes(PositiveInteger integer) => _minutes = integer;

    public Minutes(PositiveLong elapsed) =>
        _minutes = PositiveInteger.New(CalculateMinutesFromElapsedSeconds(elapsed.Read()));

    public PositiveInteger Read() => _minutes;

    private static int CalculateMinutesFromElapsedSeconds(long seconds) =>
        (int)((seconds % 3600) / 60);

    public static implicit operator int(Minutes minutes) => minutes._minutes;
}
