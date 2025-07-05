using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects.WorkingStatistics;
using RemTech.ParsersManagement.Core.Common.ValueObjects.WorkingTimes;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects;

public sealed class ParserLinkStatistic
{
    private WorkingStatistic _statistic;

    public ParserLinkStatistic(WorkingStatistic statistic) => _statistic = statistic;

    public void ApplyElapsed(PositiveLong elapsed) => _statistic = _statistic.ApplyElapsed(elapsed);

    public void IncreaseProcessed() => _statistic = _statistic.IncreaseProcessed();

    public void Reset() => _statistic = _statistic.Reset();

    public WorkingTime WorkedTime() => _statistic.WorkedTime();

    public IncrementableNumber ProcessedAmount() => _statistic.ProcessedAmount();

    public static ParserLinkStatistic operator ++(ParserLinkStatistic statistic)
    {
        return new ParserLinkStatistic(statistic._statistic.IncreaseProcessed());
    }

    public static ParserLinkStatistic operator +(
        ParserLinkStatistic statistic,
        PositiveLong elapsed
    )
    {
        return new ParserLinkStatistic(statistic._statistic.ApplyElapsed(elapsed));
    }
}
