using ParsersControl.Core.Common;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserLinks.Models;

public sealed class SubscribedParserLink
{
	public SubscribedParserLink(SubscribedParserLink link)
		: this(link.ParserId, link.Id, link.UrlInfo, link.Statistics, link.Active) { }

	private SubscribedParserLink(
		SubscribedParserId parserId,
		SubscribedParserLinkId id,
		SubscribedParserLinkUrlInfo urlInfo,
		ParsingStatistics statistics,
		bool active
	) => (ParserId, Id, UrlInfo, Active, Statistics) = (parserId, id, urlInfo, active, statistics);

	private SubscribedParserLink(SubscribedParser parser, SubscribedParserLinkUrlInfo urlInfo)
		: this(parser.Id, SubscribedParserLinkId.New(), urlInfo, ParsingStatistics.New(), false) { }

	public SubscribedParserId ParserId { get; private set; }
	public SubscribedParserLinkId Id { get; private set; }
	public SubscribedParserLinkUrlInfo UrlInfo { get; private set; }
	public ParsingStatistics Statistics { get; private set; }
	public bool Active { get; private set; }

	public void ResetWorkTime() => Statistics = Statistics.ResetWorkTime();

	public Result AddParsedCount(int count)
	{
		Result<ParsingStatistics> updated = Statistics.IncreaseParsedCount(count);
		if (updated.IsFailure)
			return Result.Failure(updated.Error);
		Statistics = updated.Value;
		return Result.Success();
	}

	public Result AddWorkTime(long totalElapsedSeconds)
	{
		Result<ParsingStatistics> updated = Statistics.AddWorkTime(totalElapsedSeconds);
		if (updated.IsFailure)
			return Result.Failure(updated.Error);
		Statistics = updated.Value;
		return Result.Success();
	}

	public void Enable()
	{
		if (Active)
			return;
		Active = true;
	}

	public void Disable()
	{
		if (!Active)
			return;
		Active = false;
	}

	public Result<SubscribedParserLink> Edit(string? otherName, string? otherUrl)
	{
		Result<SubscribedParserLinkUrlInfo> copy = UrlInfo.Copy();

		if (!string.IsNullOrWhiteSpace(otherName))
			copy = copy.Value.Rename(otherName);

		if (!string.IsNullOrWhiteSpace(otherUrl))
			copy = copy.Value.ChangeUrl(otherUrl);

		if (copy.IsFailure)
			return Result.Failure<SubscribedParserLink>(copy.Error);
		UrlInfo = copy.Value;
		return this;
	}

	public static SubscribedParserLink New(SubscribedParser parser, SubscribedParserLinkUrlInfo urlInfo) =>
		new(parser, urlInfo);

	public static SubscribedParserLink Create(
		SubscribedParserId parserId,
		SubscribedParserLinkId id,
		SubscribedParserLinkUrlInfo urlInfo,
		ParsingStatistics statistics,
		bool active
	) => new(parserId, id, urlInfo, statistics, active);

	public static SubscribedParserLink CreateCopy(SubscribedParserLink link) => new(link);
}
