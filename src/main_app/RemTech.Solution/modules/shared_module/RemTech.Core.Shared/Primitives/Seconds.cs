namespace RemTech.Core.Shared.Primitives;

public readonly record struct Seconds
{
    private readonly PositiveInteger _seconds = new();

    public Seconds(PositiveInteger seconds) => _seconds = seconds;

    public Seconds(PositiveLong elapsed) =>
        _seconds = PositiveInteger.New(CalculateSecondsFromElapsedSeconds(elapsed.Read()));

    public PositiveInteger Read() => _seconds;

    private static int CalculateSecondsFromElapsedSeconds(long seconds) => (int)(seconds % 60);

    public static implicit operator int(Seconds seconds) => seconds._seconds;
}
