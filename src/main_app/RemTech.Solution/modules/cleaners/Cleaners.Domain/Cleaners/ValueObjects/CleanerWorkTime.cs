using RemTech.Core.Shared.Result;

namespace Cleaners.Domain.Cleaners.ValueObjects;

public readonly record struct CleanerWorkTime
{
    public int ElapsedSeconds { get; }
    public int ElapsedMinutes { get; }
    public int ElapsedHours { get; }

    public CleanerWorkTime()
    {
        ElapsedSeconds = 0;
        ElapsedMinutes = 0;
        ElapsedHours = 0;
    }

    private CleanerWorkTime(int elapsedSeconds, int elapsedMinutes, int elapsedHours)
    {
        ElapsedSeconds = elapsedSeconds;
        ElapsedMinutes = elapsedMinutes;
        ElapsedHours = elapsedHours;
    }

    public static Status<CleanerWorkTime> Create(int seconds, int minutes, int hours)
    {
        if (seconds < 0)
            return Error.Validation("Время работы чистильщика не может быть отрицательным.");

        if (minutes < 0)
            return Error.Validation("Время работы чистильщика не может быть отрицательным.");

        if (hours < 0)
            return Error.Validation("Время работы чистильщика не может быть отрицательным.");

        return new CleanerWorkTime(seconds, minutes, hours);
    }

    public static Status<CleanerWorkTime> Create(long totalElapsedSeconds)
    {
        if (totalElapsedSeconds < 0)
            return Error.Validation("Время работы чистильщика не может быть отрицательным.");

        int seconds = (int)(totalElapsedSeconds / 3600);
        int minutes = (int)(totalElapsedSeconds % 3600 / 60);
        int hours = (int)(totalElapsedSeconds % 3600 % 60);

        return Create(seconds, minutes, hours);
    }
}
