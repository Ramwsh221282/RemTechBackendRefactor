using System.Text;
using System.Text.Json;
using Cleaner.WebApi.Models;

namespace Cleaner.WebApi.Messages;

public sealed record CleanerWorkStarted(
    Guid Id,
    int CleanedAmount,
    string State,
    DateTime LastRun,
    DateTime NextRun,
    int WaitDays,
    int Seconds,
    int Minutes,
    int Hours,
    int ItemsDateDayThreshold
)
{
    public static CleanerWorkStarted FromMessageBody(ReadOnlyMemory<byte> body)
    {
        string json = Encoding.UTF8.GetString(body.ToArray());
        using JsonDocument document = JsonDocument.Parse(json);
        string content = document.RootElement.GetProperty("content").GetRawText();
        return JsonSerializer.Deserialize<CleanerWorkStarted>(content)!;
    }

    public WorkingCleaner ToWorkingCleaner()
    {
        return new WorkingCleaner
        {
            Id = Id,
            CleanedAmount = CleanedAmount,
            State = State,
            TotalElapsedSeconds = 0,
            ItemsDateDayThreshold = ItemsDateDayThreshold,
        };
    }
}
