using RemTech.ParsersManagement.Core.Common.Primitives;

namespace RemTech.ParsersManagement.Core.Common.ValueObjects.WorkingTimes;

public sealed class Seconds
{
    private readonly PositiveInteger _seconds;

    public Seconds(PositiveInteger seconds) => _seconds = seconds;

    public Seconds(PositiveLong elapsed) =>
        _seconds = PositiveInteger.New(CalculateSecondsFromElapsedSeconds(elapsed.Read()));

    public PositiveInteger Read() => _seconds;

    private static int CalculateSecondsFromElapsedSeconds(long seconds) => (int)(seconds % 60);
}
