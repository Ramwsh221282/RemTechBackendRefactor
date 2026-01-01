using ParsersControl.Core.Parsers.Models;

namespace ParsersControl.WebApi.ResponseModels;

public sealed class ParserResponseModel
{
    public required Guid Id { get; init; }
    public required string Type { get; init; }
    public required string Domain { get; init; }
    public required string State { get; init; }
    public required DateTime? StartedAt { get; init; }
    public required int Processed { get; init; }
    public required int ElapsedHours { get; init; }
    public required int ElapsedMinutes { get; init; }
    public required int ElapsedSeconds { get; init; }
    public required int? WaitDays { get; init; }
    public required DateTime? NextRun { get; init; }
    public required DateTime? FinishedAt { get; init; }

    public static IEnumerable<ParserResponseModel> ConvertFrom(IEnumerable<SubscribedParser> parsers)
    {
        return parsers.Select(ConvertFrom).ToArray();
    }
    
    public static ParserResponseModel ConvertFrom(SubscribedParser parser)
    {
        return new ParserResponseModel()
        {
            Id = parser.Id.Value,
            Type = parser.Identity.ServiceType,
            Domain = parser.Identity.DomainName,
            State = parser.State.Value,
            StartedAt = parser.Schedule.StartedAt,
            Processed = parser.Statistics.ParsedCount.Value,
            ElapsedHours = parser.Statistics.WorkTime.Hours,
            ElapsedMinutes = parser.Statistics.WorkTime.Minutes,
            ElapsedSeconds = parser.Statistics.WorkTime.Seconds,
            WaitDays = parser.Schedule.WaitDays,
            NextRun = parser.Schedule.NextRun,
            FinishedAt = parser.Schedule.FinishedAt
        };
    }
}