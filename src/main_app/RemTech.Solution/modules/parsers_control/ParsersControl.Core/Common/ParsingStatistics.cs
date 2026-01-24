using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Common;

public sealed record ParsingStatistics
{
	public ParsingStatistics(ParsingWorkTime workTime, ParsedCount parsedCount)
	{
		WorkTime = workTime;
		ParsedCount = parsedCount;
	}

	public ParsingWorkTime WorkTime { get; private init; }
	public ParsedCount ParsedCount { get; private init; }

	public Result<ParsingStatistics> IncreaseParsedCount(int amount)
	{
		Result<ParsedCount> updated = ParsedCount.Add(amount);
		return updated.IsFailure ? updated.Error : this with { ParsedCount = updated.Value };
	}

	public Result<ParsingStatistics> AddWorkTime(long totalElapsedSeconds)
	{
		Result<ParsingWorkTime> updated = ParsingWorkTime.FromTotalElapsedSeconds(totalElapsedSeconds);
		return updated.IsFailure ? updated.Error : this with { WorkTime = updated.Value };
	}

	public ParsingStatistics ResetWorkTime() => this with { WorkTime = ParsingWorkTime.New() };

	public ParsingStatistics ResetParsedCount() => this with { ParsedCount = ParsedCount.New() };

	public static ParsingStatistics New()
	{
		ParsingWorkTime workTime = ParsingWorkTime.New();
		ParsedCount parsedCount = ParsedCount.New();
		return new ParsingStatistics(workTime, parsedCount);
	}
}
