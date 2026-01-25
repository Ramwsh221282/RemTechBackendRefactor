using ParsersControl.Core.Parsers.Models;

namespace ParsersControl.Infrastructure.Parsers.Queries.GetParsers;

public sealed record ParserResponse(
    Guid Id,
    string Domain,
    string ServiceType,
    DateTime? FinishedAt,
    DateTime? StartedAt,
    DateTime? NextRun,
    int? WaitDays,
    string State,
    int ParsedCount,
    int ElapsedHours,
    int ElapsedSeconds,
    int ElapsedMinutes,
    IEnumerable<ParserLinkResponse> Links
)
{
    public static ParserResponse Create(SubscribedParser parser) =>
        new(
            parser.Id.Value,
            parser.Identity.DomainName,
            parser.Identity.ServiceType,
            parser.Schedule.FinishedAt,
            parser.Schedule.StartedAt,
            parser.Schedule.NextRun,
            parser.Schedule.WaitDays,
            parser.State.Value,
            parser.Statistics.ParsedCount.Value,
            parser.Statistics.WorkTime.Hours,
            parser.Statistics.WorkTime.Minutes,
            parser.Statistics.WorkTime.Seconds,
            parser.Links.Select(ParserLinkResponse.Create)
        );
}
