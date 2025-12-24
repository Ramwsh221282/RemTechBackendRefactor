using ParsersControl.Core.Parsers.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Models;

public sealed class SubscribedParser : ISubscribedParser
{
    public SubscribedParser(
        SubscribedParserId id,
        SubscribedParserIdentity identity,
        SubscribedParserStatistics statistics,
        SubscribedParserState state,
        SubscribedParserSchedule schedule
    )
    {
        Id = id;
        Identity = identity;
        Statistics = statistics;
        State = state;
        Schedule = schedule;        
    }
    
    public SubscribedParserId Id { get; }
    public SubscribedParserIdentity Identity { get; }
    public SubscribedParserStatistics Statistics { get; private set; }
    public SubscribedParserState State { get; private set; }
    public SubscribedParserSchedule Schedule { get; private set; }

    public Result<SubscribedParser> AddParserAmount(int amount)
    {
        if (!State.IsWorking()) 
            return Error.Conflict($"Для добавления количества обработанных данных парсер должен быть в состоянии {SubscribedParserState.Working.Value}.");
        Result<SubscribedParserStatistics> updated = Statistics.IncreaseParsedCount(amount);
        if (updated.IsFailure) return updated.Error;
        Statistics = updated.Value;
        return this;
    }

    public Result<SubscribedParser> Enable()
    {
        if (State.IsWorking())
            return Error.Conflict($"Парсер в состоянии {State.Value}. Невозможно включить.");
        if (State.IsSleeping())
            return Error.Conflict($"Парсер в состоянии {State.Value}. Невозможно включить.");
        State = SubscribedParserState.Sleeping;
        return this;
    }
    
    public Result<SubscribedParser> AddWorkTime(long totalElapsedSeconds)
    {
        if (!State.IsWorking()) 
            return Error.Conflict($"Для добавления времени работы парсер должен быть в состоянии {SubscribedParserState.Working.Value}.");
        Result<SubscribedParserStatistics> updated = Statistics.AddWorkTime(totalElapsedSeconds);
        if (updated.IsFailure) return updated.Error;
        Statistics = updated.Value;
        return this;
    }

    public SubscribedParser ResetWorkTime()
    {
        Statistics = Statistics.ResetWorkTime();
        return this;
    }
    
    public SubscribedParser ResetParsedCount()
    {
        Statistics = Statistics.ResetParsedCount();
        return this;
    }

    public Result<SubscribedParser> StartWork()
    {
        if (State.IsWorking())
            return Error.Conflict($"Парсер уже в состоянии {State.Value}.");
        if (State.IsDisabled())
            return Error.Conflict($"Парсер в состоянии {State.Value}. Невозможно начать работу.");
        State = SubscribedParserState.Working;
        Schedule = Schedule.WithStartedAt(DateTime.UtcNow);
        return this;
    }

    public Result<SubscribedParser> ChangeScheduleWaitDays(int waitDays)
    {
        if (State.IsWorking())
            return Error.Conflict($"Парсер в состоянии {State.Value}. Невозможно изменить дни ожидания.");
        Result<SubscribedParserSchedule> updated = Schedule.WithWaitDays(waitDays);
        if (updated.IsFailure) return updated.Error;
        Schedule = updated.Value;
        return this;
    }

    public Result<SubscribedParser> ChangeScheduleNextRun(DateTime nextRun)
    {
        if (State.IsWorking())
            return Error.Conflict($"Парсер не в состоянии {State.Value}. Невозможно изменить дату следующего запуска.");
        Result<SubscribedParserSchedule> updated = Schedule.WithNextRun(nextRun);
        if (updated.IsFailure) return updated.Error;
        Schedule = updated.Value;
        return this;
    }

    public SubscribedParser Disable()
    {
        State = SubscribedParserState.Disabled;
        Schedule = Schedule.WithFinishedAt(DateTime.UtcNow);
        return this;
    }
    
    public Result<SubscribedParser> FinishWork()
    {
        if (!State.IsWorking())
            return Error.Conflict($"Парсер не в состоянии {SubscribedParserState.Working.Value}, чтобы завершить работу.");
        State = SubscribedParserState.Sleeping;
        Result<SubscribedParserSchedule> updated = Schedule.WithFinishedAt(DateTime.UtcNow);
        if (updated.IsFailure) return updated.Error;
        Schedule = updated.Value;
        return this;
    }

    public static async Task<Result<SubscribedParser>> CreateNew(
        SubscribedParserId id, 
        SubscribedParserIdentity identity, 
        ISubscribedParsersRepository repository,
        CancellationToken ct = default)
    {
        if (await repository.Exists(identity, ct: ct)) 
            return Error.Conflict($"Парсер для домена {identity.DomainName} и типа {identity.ServiceType} уже существует.");
        
        SubscribedParserStatistics statistics = SubscribedParserStatistics.New();
        SubscribedParserState state = SubscribedParserState.Disabled;
        SubscribedParserSchedule schedule = SubscribedParserSchedule.New();
        SubscribedParser parser = new SubscribedParser(id, identity, statistics, state, schedule);
        await repository.Add(parser, ct: ct);
        return parser;
    }
}