using ParsersControl.Core.Common;
using ParsersControl.Core.Contracts;
using ParsersControl.Core.ParserLinks.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Models;

public sealed class SubscribedParser : ISubscribedParser
{
    public SubscribedParser(SubscribedParser parser, IEnumerable<SubscribedParserLink> links)
        : this(parser.Id, parser.Identity, parser.Statistics, parser.State, parser.Schedule, [..links]) { }
    
    public SubscribedParser(
        SubscribedParserId id,
        SubscribedParserIdentity identity,
        ParsingStatistics statistics,
        SubscribedParserState state,
        SubscribedParserSchedule schedule,
        IReadOnlyList<SubscribedParserLink> links
    ) : this(id, identity, statistics, state, schedule) => Links = [..links];
    
    public SubscribedParser(SubscribedParserId id,
        SubscribedParserIdentity identity,
        ParsingStatistics statistics,
        SubscribedParserState state,
        SubscribedParserSchedule schedule) => 
        (Id, Identity, Statistics, State, Schedule, Links) = (id, identity, statistics, state, schedule, []);

    public IReadOnlyList<SubscribedParserLink> Links { get; private set; }
    public SubscribedParserId Id { get; }
    public SubscribedParserIdentity Identity { get; }
    public ParsingStatistics Statistics { get; private set; }
    public SubscribedParserState State { get; private set; }
    public SubscribedParserSchedule Schedule { get; private set; }

    public Result<SubscribedParser> AddParserAmount(int amount)
    {
        if (!State.IsWorking()) 
            return Error.Conflict($"Для добавления количества обработанных данных парсер должен быть в состоянии {SubscribedParserState.Working.Value}.");
        Result<ParsingStatistics> updated = Statistics.IncreaseParsedCount(amount);
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
        Result<ParsingStatistics> updated = Statistics.AddWorkTime(totalElapsedSeconds);
        if (updated.IsFailure) return updated.Error;
        Statistics = updated.Value;
        return this;
    }

    public Result<SubscribedParserLink> AddLink(SubscribedParserLinkUrlInfo urlInfo)
    {
        if (State.IsWorking())
            return Error.Conflict($"Для добавления ссылки парсер должен быть в состоянии {SubscribedParserState.Disabled.Value} или {SubscribedParserState.Sleeping.Value}.");
        if (ContainsLinkWithName(urlInfo))
            return Error.Conflict($"Парсер уже содержит ссылку с именем {urlInfo.Name}.");
        if (ContainsLinkWithUrl(urlInfo))
            return Error.Conflict($"Парсер уже содержит ссылку с адресом {urlInfo.Url}.");
        return SubscribedParserLink.New(this, urlInfo);
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

    public Result<SubscribedParserLink> FindLink(Func<SubscribedParserLinkUrlInfo, bool> predicate)
    {
        SubscribedParserLink? link = Links.FirstOrDefault(l => predicate(l.UrlInfo));
        if (link is null) return Error.NotFound($"Ссылка не найдена.");
        return link;
    }

    public Result<SubscribedParserLink> FindLink(Guid id)
    {
        SubscribedParserLink? link = Links.FirstOrDefault(l => l.Id.Value == id);
        if (link is null) return Error.NotFound($"Ссылка не найдена.");
        return link;
    }

    public Result<SubscribedParserLink> FindLink(SubscribedParserLinkId id)
    {
        SubscribedParserLink? link = Links.FirstOrDefault(l => l.Id == id);
        if (link is null) return Error.NotFound($"Ссылка не найдена.");
        return link;
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
        ParsingStatistics statistics = ParsingStatistics.New();
        SubscribedParserState state = SubscribedParserState.Disabled;
        SubscribedParserSchedule schedule = SubscribedParserSchedule.New();
        SubscribedParser parser = new SubscribedParser(id, identity, statistics, state, schedule);
        await repository.Add(parser, ct: ct);
        return parser;
    }

    private bool ContainsLinkWithName(SubscribedParserLinkUrlInfo urlInfo)
    {
        return Links.Any(link => link.UrlInfo.Name == urlInfo.Name);
    }

    private bool ContainsLinkWithUrl(SubscribedParserLinkUrlInfo urlInfo)
    {
        return Links.Any(link => link.UrlInfo.Url == urlInfo.Url);
    }
}