using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects.WorkingStatistics;
using RemTech.ParsersManagement.Core.Common.ValueObjects.WorkingTimes;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

public sealed class ParserStatistic
{
    private WorkingStatistic _statistic;

    public ParserStatistic(WorkingStatistic statistic) => _statistic = statistic;

    public ParserStatistic() => _statistic = new WorkingStatistic();

    public void ApplyElapsed(PositiveLong elapsed) => _statistic = _statistic.ApplyElapsed(elapsed);

    public void IncreaseProcessed() => _statistic = _statistic.IncreaseProcessed();

    public void Reset() => _statistic = _statistic.Reset();

    public WorkingTime WorkedTime() => _statistic.WorkedTime();

    public IncrementableNumber ProcessedAmount() => _statistic.ProcessedAmount();

    public static ParserStatistic operator ++(ParserStatistic statistic) =>
        new(statistic._statistic.IncreaseProcessed());

    public static ParserStatistic operator +(ParserStatistic statistic, PositiveLong elapsed) =>
        new(statistic._statistic.ApplyElapsed(elapsed));
}
