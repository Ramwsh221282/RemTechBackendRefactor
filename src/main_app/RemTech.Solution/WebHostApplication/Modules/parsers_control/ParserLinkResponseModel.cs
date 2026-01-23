using ParsersControl.Core.ParserLinks.Models;

namespace WebHostApplication.Modules.parsers_control;

public sealed class ParserLinkResponseModel
{
	public required Guid Id { get; init; }
	public required string Name { get; init; }
	public required string Url { get; init; }
	public required int ParsedCount { get; init; }
	public required long WorkTime { get; init; }
	public required bool IsActive { get; init; }
	public required int Hours { get; init; }
	public required int Minutes { get; init; }
	public required int Seconds { get; init; }

	public static ParserLinkResponseModel ConvertFrom(SubscribedParserLink link) =>
		new()
		{
			Id = link.Id.Value,
			Name = link.UrlInfo.Name,
			Url = link.UrlInfo.Url,
			ParsedCount = link.Statistics.ParsedCount.Value,
			WorkTime = link.Statistics.WorkTime.TotalElapsedSeconds,
			IsActive = link.Active,
			Hours = link.Statistics.WorkTime.Hours,
			Minutes = link.Statistics.WorkTime.Minutes,
			Seconds = link.Statistics.WorkTime.Seconds,
		};

	public static IEnumerable<ParserLinkResponseModel> ConvertFrom(IEnumerable<SubscribedParserLink> links) =>
		[.. links.Select(ConvertFrom)];
}
