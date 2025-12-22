using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Models;

public readonly record struct SubscribedParserSchedule
{
    private const int MaxWaitDaysAmount = 7;
    public DateTime? StartedAt { get; private init; }
    public DateTime? FinishedAt { get; private init; }
    public DateTime? NextRun { get; private init; }
    public int? WaitDays { get; private init; }

    public SubscribedParserSchedule()
    {
        StartedAt = null;
        FinishedAt = null;
        WaitDays = null;
    }

    private SubscribedParserSchedule(
        DateTime? startedAt, 
        DateTime? finishedAt,
        DateTime? nextRun,
        int? waitDays)
    {
        StartedAt = startedAt;
        FinishedAt = finishedAt;
        NextRun = nextRun;
        WaitDays = waitDays;
    }

    public Result<SubscribedParserSchedule> WithStartedAt(DateTime startedAt)
    {
        return Create(startedAt, FinishedAt, NextRun, WaitDays);
    }

    public SubscribedParserSchedule WithNextRun(DateTime nextRun)
    {
        return Create(StartedAt, FinishedAt, nextRun, WaitDays);
    }
    
    public Result<SubscribedParserSchedule> WithFinishedAt(DateTime finishedAt)
    {
        Result<SubscribedParserSchedule> result = Create(StartedAt, finishedAt, NextRun, WaitDays);
        if (result.IsFailure) return result.Error;
        return result.Value.AdjustNextRun();
    }

    public Result<SubscribedParserSchedule> WithWaitDays(int waitDays)
    {
        return Create(StartedAt, FinishedAt, NextRun, waitDays);
    }

    private SubscribedParserSchedule AdjustNextRun()
    {
        if (WaitDays == null || FinishedAt == null)
            return this;
        DateTime newNextRun = FinishedAt.Value.AddDays(WaitDays.Value);
        return Create(StartedAt, FinishedAt, newNextRun, WaitDays);
    }
    
    public static SubscribedParserSchedule New()
    {
        return new() { WaitDays = 1 };
    }

    public static Result<SubscribedParserSchedule> Create(
        DateTime? startedAt,
        DateTime? finishedAt,
        DateTime? nextRun,
        int? waitDays)
    {
        if (waitDays != null)
        {
            if (waitDays.Value < 1 || waitDays.Value > MaxWaitDaysAmount)
                return Error.Validation($"Дни ожидания не могут быть менее 1 или более {MaxWaitDaysAmount}");
        }

        if (startedAt != null && finishedAt != null)
        {
            if (finishedAt.Value < startedAt.Value)
                return Error.Validation("Дата окончания не может быть раньше даты начала.");
        }
        
        return new SubscribedParserSchedule(startedAt, finishedAt, nextRun, waitDays);        
    }
}