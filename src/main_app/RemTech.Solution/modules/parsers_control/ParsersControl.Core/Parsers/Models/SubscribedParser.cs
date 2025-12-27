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

    public Result<IEnumerable<SubscribedParserLink>> AddLinks(IEnumerable<SubscribedParserLinkUrlInfo> urlInfos)
    {
        if (State.IsWorking()) 
            return Error.Conflict($"Для добавления ссылок парсер должен быть в состоянии {SubscribedParserState.Disabled.Value} или {SubscribedParserState.Sleeping.Value}.");
        List<SubscribedParserLink> newLinks = new();
        foreach (SubscribedParserLinkUrlInfo info in urlInfos)
        {
            SubscribedParserLink link = SubscribedParserLink.New(this, info);
            if (ContainsLinkWithName(link))
                return Error.Conflict($"Парсер уже содержит ссылку с именем {link.UrlInfo.Name}.");
            if (ContainsLinkWithUrl(link))
                return Error.Conflict($"Парсер уже содержит ссылку с адресом {link.UrlInfo.Url}.");
            newLinks.Add(link);
        }
        Links = [..Links, ..newLinks];
        return newLinks;
    }

    public Result<SubscribedParser> AddParserAmount(int amount)
    {
        if (!State.IsWorking()) 
            return Error.Conflict($"Для добавления количества обработанных данных парсер должен быть в состоянии {SubscribedParserState.Working.Value}.");
        Result<ParsingStatistics> updated = Statistics.IncreaseParsedCount(amount);
        if (updated.IsFailure) return updated.Error;
        Statistics = updated.Value;
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
        SubscribedParserLink link = SubscribedParserLink.New(this, urlInfo);
        if (ContainsLinkWithName(link))
            return Error.Conflict($"Парсер уже содержит ссылку с именем {link.UrlInfo.Name}.");
        if (ContainsLinkWithUrl(link))
            return Error.Conflict($"Парсер уже содержит ссылку с адресом {link.UrlInfo.Url}.");
        AddLinkToCollection(link);
        return link;
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
        (bool isWorking, bool isDisabled, bool hasNoLinks, bool allLinksInactive) =
            (State.IsWorking(), State.IsDisabled(), HasNoLinks(), AllLinksAreInactive());

        Func<Result<SubscribedParser>> operation = (isWorking, isDisabled, hasNoLinks, allLinksInactive) switch
        {
            (false, false, false, false) => () =>
            {
                State = SubscribedParserState.Working;
                Schedule = Schedule.WithStartedAt(DateTime.UtcNow);
                return this;
            },
            (_, _, true, _) => () => Error.Conflict($"Парсер не содержит ссылок."),
            (_, _, _, true) => () => Error.Conflict($"Парсер не содержит активных ссылок."),
            (_, true, _, _) => () => Error.Conflict($"Парсер в состоянии {State.Value}. Невозможно начать работу."),
            (true, _, _, _) => () => Error.Conflict($"Парсер уже в состоянии {State.Value}."),
        };

        return operation();
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
    
    public Result<SubscribedParserLink> ChangeLinkActivity(SubscribedParserLink link, bool isActive)
    {
        if (State.IsWorking())
            return Error.Conflict($"Парсер в состоянии {State.Value}. Невозможно изменить активность ссылки.");
        if (!BelongsToParser(link)) 
            return Error.Conflict($"Ссылка {link.Id.Value} не принадлежит парсеру {Id.Value}.");
        if (isActive) link.Enable();
        else link.Disable();
        return link;
    }

    public SubscribedParser Disable()
    {
        State = SubscribedParserState.Disabled;
        Schedule = Schedule.WithFinishedAt(DateTime.UtcNow);
        return this;
    }

    public Result<SubscribedParser> FinishWork(long totalElapsedSeconds)
    {
        if (!State.IsWorking())
            return Error.Conflict($"Парсер не в состоянии {SubscribedParserState.Working.Value}, чтобы завершить работу.");
        Result<ParsingStatistics> update = Statistics.AddWorkTime(totalElapsedSeconds);
        if (update.IsFailure) return update.Error;
        Statistics = update.Value;
        State = SubscribedParserState.Sleeping;
        Schedule = Schedule.WithFinishedAt(DateTime.UtcNow);
        return this;
    }

    public Result<SubscribedParser> PermantlyEnable()
    {
        if (HasNoLinks()) return Error.Conflict($"Парсер не содержит ссылок.");
        State = SubscribedParserState.Working;
        Schedule = Schedule.WithStartedAt(DateTime.UtcNow);
        return this;
    }

    public SubscribedParser PermantlyDisable()
    {
        State = SubscribedParserState.Disabled;
        return this;
    }

    public Result<SubscribedParserLink> RemoveLink(SubscribedParserLink link)
    {
        if (!State.IsDisabled() && !State.IsSleeping())
            return Error.Conflict($"Для удаления ссылки парсер должен быть в состоянии {SubscribedParserState.Disabled.Value} или {SubscribedParserState.Sleeping.Value}.");
        if (!BelongsToParser(link))
            return Error.Conflict($"Ссылка {link.Id.Value} не принадлежит парсеру {Id.Value}.");
        Links = Links.Where(l => l.Id != link.Id).ToArray();
        return link;
    }

    public Result<SubscribedParserLink> AddLinkParsedAmount(SubscribedParserLink link, int count)
    {
        if (!State.IsWorking())
            return Error.Conflict($"Для добавления количества обработанных данных парсер должен быть в состоянии {SubscribedParserState.Working.Value}.");
        if (!BelongsToParser(link))
            return Error.Conflict($"Ссылка {link.Id.Value} не принадлежит парсеру {Id.Value}.");
        link.AddParsedCount(count);
        return link;
    }

    public Result<SubscribedParserLink> AddLinkWorkTime(SubscribedParserLink link, long totalElapsedSeconds)
    {
        if (!State.IsWorking())
            return Error.Conflict($"Для добавления времени работы парсер должен быть в состоянии {SubscribedParserState.Working.Value}.");
        if (!BelongsToParser(link))
            return Error.Conflict($"Ссылка {link.Id.Value} не принадлежит парсеру {Id.Value}.");
        link.AddWorkTime(totalElapsedSeconds);
        return link;
    }

    public Result<SubscribedParserLink> EditLink(SubscribedParserLink link, string? newName, string? newUrl)
    {
        if (!BelongsToParser(link))
            return Error.Conflict($"Ссылка {link.Id.Value} не принадлежит парсеру {Id.Value}.");
        Result<SubscribedParserLink> editResult = link.Edit(newName, newUrl);
        if (editResult.IsFailure) return Result.Failure<SubscribedParserLink>(editResult.Error);
        if (ContainsLinkWithName(editResult.Value))
            return Error.Conflict($"Парсер уже содержит ссылку с именем {editResult.Value.UrlInfo.Name}.");
        if (ContainsLinkWithUrl(editResult.Value))
            return Error.Conflict($"Парсер уже содержит ссылку с адресом {editResult.Value.UrlInfo.Url}.");
        return editResult;
    }

    public Result<SubscribedParser> Enable()
    {
        if (State.IsSleeping())
            return Error.Conflict($"Парсер уже в состоянии {State.Value}.");
        State = SubscribedParserState.Sleeping;
        Schedule = Schedule.WithStartedAt(DateTime.UtcNow);
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

    private bool BelongsToParser(SubscribedParserLink link) =>
        link.ParserId == Id;
    
    private bool ContainsLinkWithName(SubscribedParserLink link) =>
        Links.Any(l => l.UrlInfo.Name == link.UrlInfo.Name && l.Id != link.Id);

    private void AddLinkToCollection(SubscribedParserLink link) => Links = [..Links, link];
    
    private bool ContainsLinkWithUrl(SubscribedParserLink link) =>
        Links.Any(l => l.UrlInfo.Url == link.UrlInfo.Url && l.Id != link.Id);

    private bool HasNoLinks()
    {
        return Links.Count == 0;
    }

    private bool AllLinksAreInactive()
    {
        return Links.All(l => l.Active == false);
    }
}