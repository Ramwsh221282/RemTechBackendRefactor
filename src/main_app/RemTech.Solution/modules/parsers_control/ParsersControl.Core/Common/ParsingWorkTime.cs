using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Common;

public sealed record ParsingWorkTime
{
	private const long InitialValue = 0;

	private ParsingWorkTime(long totalElapsedSeconds)
	{
		TotalElapsedSeconds = totalElapsedSeconds;
	}

	public long TotalElapsedSeconds { get; private init; }
	public int Hours => (int)(TotalElapsedSeconds / 3600);
	public int Minutes => (int)((TotalElapsedSeconds % 3600) / 60);
	public int Seconds => (int)(TotalElapsedSeconds % 60);

	public static ParsingWorkTime New() => new(InitialValue);

	public static Result<ParsingWorkTime> FromTotalElapsedSeconds(long totalElapsedSeconds)
	{
		return totalElapsedSeconds < InitialValue
			? Error.Validation("Общее время работы парсера не может быть отрицательным.")
			: new ParsingWorkTime(totalElapsedSeconds);
	}
}
