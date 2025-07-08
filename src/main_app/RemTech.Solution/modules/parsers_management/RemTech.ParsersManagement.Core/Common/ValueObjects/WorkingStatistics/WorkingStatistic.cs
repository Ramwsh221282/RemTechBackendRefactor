using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects.WorkingTimes;

namespace RemTech.ParsersManagement.Core.Common.ValueObjects.WorkingStatistics;

public sealed class WorkingStatistic
{
    private readonly WorkingTime _time;
    private readonly IncrementableNumber _processedAmount;

    public WorkingStatistic()
    {
        _time = new WorkingTime();
        _processedAmount = new IncrementableNumber();
    }

    public WorkingStatistic(WorkingTime time, IncrementableNumber processedAmount)
    {
        _time = time;
        _processedAmount = processedAmount;
    }

    public WorkingStatistic ApplyElapsed(PositiveLong elapsed)
    {
        WorkingTime updated = new(elapsed);
        return new WorkingStatistic(updated, _processedAmount);
    }

    public WorkingStatistic IncreaseProcessed()
    {
        IncrementableNumber increased = _processedAmount.Increase();
        return new WorkingStatistic(_time, increased);
    }

    public WorkingStatistic Reset() => new WorkingStatistic();

    public WorkingTime WorkedTime() => _time;

    public IncrementableNumber ProcessedAmount() => _processedAmount;
}
