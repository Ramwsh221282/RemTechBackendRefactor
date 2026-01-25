using ParsersControl.Core.Common;
using ParsersControl.Core.Contracts;
using ParsersControl.Core.Features.UpdateParserLinks;
using ParsersControl.Core.ParserLinks.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Models;

public sealed class SubscribedParser
{
	public SubscribedParser(SubscribedParser parser, IEnumerable<SubscribedParserLink> links)
		: this(parser.Id, parser.Identity, parser.Statistics, parser.State, parser.Schedule, [.. links]) { }

	public SubscribedParser(
		SubscribedParserId id,
		SubscribedParserIdentity identity,
		ParsingStatistics statistics,
		SubscribedParserState state,
		SubscribedParserSchedule schedule,
		IReadOnlyList<SubscribedParserLink> links
	)
		: this(id, identity, statistics, state, schedule) => Links = [.. links];

	public SubscribedParser(
		SubscribedParserId id,
		SubscribedParserIdentity identity,
		ParsingStatistics statistics,
		SubscribedParserState state,
		SubscribedParserSchedule schedule
	) => (Id, Identity, Statistics, State, Schedule, Links) = (id, identity, statistics, state, schedule, []);

	private SubscribedParser(SubscribedParser parser)
		: this(
			parser.Id,
			parser.Identity,
			parser.Statistics,
			parser.State,
			parser.Schedule,
			[.. parser.Links.Select(SubscribedParserLink.CreateCopy)]
		) { }

	public IReadOnlyList<SubscribedParserLink> Links { get; private set; }
	public SubscribedParserId Id { get; }
	public SubscribedParserIdentity Identity { get; }
	public ParsingStatistics Statistics { get; private set; }
	public SubscribedParserState State { get; private set; }
	public SubscribedParserSchedule Schedule { get; private set; }

	public Result<IEnumerable<SubscribedParserLink>> AddLinks(IEnumerable<SubscribedParserLinkUrlInfo> urlInfos)
	{
		if (State.IsWorking())
		{
			return Error.Conflict(
				$"Для добавления ссылок парсер должен быть в состоянии {SubscribedParserState.Disabled.Value} или {SubscribedParserState.Sleeping.Value}."
			);
		}

		List<SubscribedParserLink> newLinks = [];
		foreach (SubscribedParserLinkUrlInfo info in urlInfos)
		{
			SubscribedParserLink link = SubscribedParserLink.New(this, info);
			if (ContainsLinkWithName(link))
				return Error.Conflict($"Парсер уже содержит ссылку с именем {link.UrlInfo.Name}.");
			if (ContainsLinkWithUrl(link))
				return Error.Conflict($"Парсер уже содержит ссылку с адресом {link.UrlInfo.Url}.");
			newLinks.Add(link);
		}
		Links = [.. Links, .. newLinks];
		return newLinks;
	}

	public Result<Unit> AddParserAmount(int amount)
	{
		Result<ParsingStatistics> updated = Statistics.IncreaseParsedCount(amount);
		if (updated.IsFailure)
			return updated.Error;
		Statistics = updated.Value;
		return Unit.Value;
	}

	public Result<Unit> AddWorkTime(long totalElapsedSeconds)
	{
		if (!State.IsWorking())
		{
			return Error.Conflict(
				$"Для добавления времени работы парсер должен быть в состоянии {SubscribedParserState.Working.Value}."
			);
		}

		Result<ParsingStatistics> updated = Statistics.AddWorkTime(totalElapsedSeconds);
		if (updated.IsFailure)
			return updated.Error;
		Statistics = updated.Value;
		return Unit.Value;
	}

	public Result<SubscribedParserLink> AddLink(SubscribedParserLinkUrlInfo urlInfo)
	{
		if (State.IsWorking())
		{
			return Error.Conflict(
				$"Для добавления ссылки парсер должен быть в состоянии {SubscribedParserState.Disabled.Value} или {SubscribedParserState.Sleeping.Value}."
			);
		}

		SubscribedParserLink link = SubscribedParserLink.New(this, urlInfo);
		if (ContainsLinkWithName(link))
			return Error.Conflict($"Парсер уже содержит ссылку с именем {link.UrlInfo.Name}.");
		if (ContainsLinkWithUrl(link))
			return Error.Conflict($"Парсер уже содержит ссылку с адресом {link.UrlInfo.Url}.");
		AddLinkToCollection(link);
		return link;
	}

	public Result<Unit> AddLinkIgnoringStatePolitics(SubscribedParserLink link)
	{
		if (ContainsLinkWithName(link))
			return Error.Conflict($"Парсер уже содержит ссылку с именем {link.UrlInfo.Name}.");
		if (ContainsLinkWithUrl(link))
			return Error.Conflict($"Парсер уже содержит ссылку с адресом {link.UrlInfo.Url}.");
		AddLinkToCollection(link);
		return Result.Success(Unit.Value);
	}

	public void ResetWorkTime() => Statistics = Statistics.ResetWorkTime();

	public void ResetParsedCount() => Statistics = Statistics.ResetParsedCount();

	public Result<Unit> StartWaiting()
	{
		if (State.IsWorking())
			return Error.Conflict($"Парсер в состоянии {State.Value}. Невозможно начать ожидание.");
		if (HasNoLinks())
			return Error.Conflict($"Парсер не содержит ссылок.");
		if (AllLinksAreInactive())
			return Error.Conflict($"Парсер не содержит активных ссылок.");
		Result<int> waitDays = GetSpecifiedWaitDays();
		if (waitDays.IsFailure)
			return waitDays.Error;
		State = SubscribedParserState.Sleeping;
		Schedule = Schedule.WithNextRun(DateTime.UtcNow.AddDays(waitDays.Value));
		return Unit.Value;
	}

	public Result<Unit> StartWork()
	{
		(bool isWorking, bool isDisabled, bool hasNoLinks, bool allLinksInactive) = (
			State.IsWorking(),
			State.IsDisabled(),
			HasNoLinks(),
			AllLinksAreInactive()
		);

		Func<Result<Unit>> operation = (isWorking, isDisabled, hasNoLinks, allLinksInactive) switch
		{
			(false, false, false, false) => () =>
			{
				State = SubscribedParserState.Working;
				Schedule = Schedule.WithStartedAt(DateTime.UtcNow);
				return Unit.Value;
			},
			(_, _, true, _) => () => Error.Conflict($"Парсер не содержит ссылок."),
			(_, _, _, true) => () => Error.Conflict($"Парсер не содержит активных ссылок."),
			(_, true, _, _) => () => Error.Conflict($"Парсер в состоянии {State.Value}. Невозможно начать работу."),
			(true, _, _, _) => () => Error.Conflict($"Парсер уже в состоянии {State.Value}."),
		};

		return operation();
	}

	public Result<Unit> ChangeScheduleWaitDays(int waitDays)
	{
		if (State.IsWorking())
			return Error.Conflict($"Парсер в состоянии {State.Value}. Невозможно изменить дни ожидания.");
		Result<SubscribedParserSchedule> updated = Schedule.WithWaitDays(waitDays);
		if (updated.IsFailure)
			return updated.Error;
		Schedule = updated.Value;
		return Unit.Value;
	}

	public Result<Unit> ChangeScheduleNextRun(DateTime nextRun)
	{
		if (State.IsWorking())
			return Error.Conflict($"Парсер не в состоянии {State.Value}. Невозможно изменить дату следующего запуска.");
		Result<SubscribedParserSchedule> updated = Schedule.WithNextRun(nextRun);
		if (updated.IsFailure)
			return updated.Error;
		Schedule = updated.Value;
		return Unit.Value;
	}

	public Result<SubscribedParserLink> FindLink(Func<SubscribedParserLinkUrlInfo, bool> predicate)
	{
		SubscribedParserLink? link = Links.FirstOrDefault(l => predicate(l.UrlInfo));
		return link is null
			? (Result<SubscribedParserLink>)Error.NotFound($"Ссылка не найдена.")
			: (Result<SubscribedParserLink>)link;
	}

	public Result<SubscribedParserLink> FindLink(Guid id)
	{
		SubscribedParserLink? link = Links.FirstOrDefault(l => l.Id.Value == id);
		return link is null
			? (Result<SubscribedParserLink>)Error.NotFound($"Ссылка не найдена.")
			: (Result<SubscribedParserLink>)link;
	}

	public Result<SubscribedParserLink> FindLink(SubscribedParserLinkId id)
	{
		SubscribedParserLink? link = Links.FirstOrDefault(l => l.Id == id);
		return link is null
			? (Result<SubscribedParserLink>)Error.NotFound($"Ссылка не найдена.")
			: (Result<SubscribedParserLink>)link;
	}

	public Result<SubscribedParserLink> ChangeLinkActivity(SubscribedParserLink link, bool isActive)
	{
		if (State.IsWorking())
			return Error.Conflict($"Парсер в состоянии {State.Value}. Невозможно изменить активность ссылки.");
		if (!BelongsToParser(link))
			return Error.Conflict($"Ссылка {link.Id.Value} не принадлежит парсеру {Id.Value}.");
		if (isActive)
			link.Enable();
		else
			link.Disable();
		return link;
	}

	public void Disable()
	{
		State = SubscribedParserState.Disabled;
		Schedule = Schedule.WithFinishedAt(DateTime.UtcNow);
	}

	public Result<IEnumerable<SubscribedParserLink>> UpdateLinks(IEnumerable<ParserLinkUpdater> updater)
	{
		if (State.IsWorking())
			return Error.Conflict($"Парсер в состоянии {State.Value}. Невозможно изменить ссылки.");

		foreach (SubscribedParserLink link in Links)
		{
			ParserLinkUpdater? linkUpdater = updater.FirstOrDefault(u => u.UpdateBelongsTo(link));
			if (linkUpdater is null)
				continue;

			Result update = linkUpdater.Update(link);
			if (update.IsFailure)
				return update.Error;
		}

		if (!AllLinksHaveUniqueName(out string[] dupNames))
			return Error.Conflict("Парсер содержит ссылки с одинаковым именем: " + string.Join(", ", dupNames));
		return !AllLinksHaveUniqueUrl(out string[] dupUrls)
			? Error.Conflict("Парсер содержит ссылки с одинаковым адресом: " + string.Join(", ", dupUrls))
			: Result.Success(Links.AsEnumerable());
	}

	public Result<Unit> FinishWork(long totalElapsedSeconds)
	{
		if (!State.IsWorking())
		{
			return Error.Conflict(
				$"Парсер не в состоянии {SubscribedParserState.Working.Value}, чтобы завершить работу."
			);
		}

		Result<ParsingStatistics> update = Statistics.AddWorkTime(totalElapsedSeconds);
		if (update.IsFailure)
			return update.Error;
		Statistics = update.Value;
		State = SubscribedParserState.Sleeping;
		Schedule = Schedule.WithFinishedAt(DateTime.UtcNow);
		return Unit.Value;
	}

	public Result<Unit> PermantlyEnable()
	{
		if (HasNoLinks())
			return Error.Conflict("Парсер не содержит ссылок.");
		if (State.IsWorking())
			return Error.Conflict($"Парсер уже в состоянии {State.Value}.");
		State = SubscribedParserState.Working;
		Schedule = Schedule.WithStartedAt(DateTime.UtcNow);
		return Unit.Value;
	}

	public void PermanentlyDisable()
	{
		State = SubscribedParserState.Disabled;
		Schedule = Schedule.WithFinishedAt(DateTime.UtcNow);
	}

	public Result<SubscribedParserLink> RemoveLink(SubscribedParserLink link)
	{
		if (!State.IsDisabled() && !State.IsSleeping())
		{
			return Error.Conflict(
				$"Для удаления ссылки парсер должен быть в состоянии {SubscribedParserState.Disabled.Value} или {SubscribedParserState.Sleeping.Value}."
			);
		}

		if (!BelongsToParser(link))
			return Error.Conflict($"Ссылка {link.Id.Value} не принадлежит парсеру {Id.Value}.");
		Links = [.. Links.Where(l => l.Id != link.Id)];
		return link;
	}

	public Result<SubscribedParserLink> AddLinkParsedAmount(SubscribedParserLink link, int count)
	{
		if (!State.IsWorking())
		{
			return Error.Conflict(
				$"Для добавления количества обработанных данных парсер должен быть в состоянии {SubscribedParserState.Working.Value}."
			);
		}

		if (!BelongsToParser(link))
			return Error.Conflict($"Ссылка {link.Id.Value} не принадлежит парсеру {Id.Value}.");
		Result result = link.AddParsedCount(count);
		return result.IsFailure ? (Result<SubscribedParserLink>)result.Error : (Result<SubscribedParserLink>)link;
	}

	public Result<SubscribedParserLink> AddLinkWorkTime(SubscribedParserLink link, long totalElapsedSeconds)
	{
		if (!State.IsWorking())
		{
			return Error.Conflict(
				$"Для добавления времени работы парсер должен быть в состоянии {SubscribedParserState.Working.Value}."
			);
		}

		if (!BelongsToParser(link))
			return Error.Conflict($"Ссылка {link.Id.Value} не принадлежит парсеру {Id.Value}.");
		Result result = link.AddWorkTime(totalElapsedSeconds);
		return result.IsFailure ? (Result<SubscribedParserLink>)result.Error : (Result<SubscribedParserLink>)link;
	}

	public Result<SubscribedParserLink> EditLink(SubscribedParserLink link, string? newName, string? newUrl)
	{
		if (!BelongsToParser(link))
			return Error.Conflict($"Ссылка {link.Id.Value} не принадлежит парсеру {Id.Value}.");
		Result<SubscribedParserLink> editResult = link.Edit(newName, newUrl);
		if (editResult.IsFailure)
			return Result.Failure<SubscribedParserLink>(editResult.Error);
		if (ContainsLinkWithName(editResult.Value))
			return Error.Conflict($"Парсер уже содержит ссылку с именем {editResult.Value.UrlInfo.Name}.");
		return ContainsLinkWithUrl(editResult.Value)
			? Error.Conflict($"Парсер уже содержит ссылку с адресом {editResult.Value.UrlInfo.Url}.")
			: editResult;
	}

	public Result<Unit> Enable()
	{
		if (State.IsSleeping())
			return Error.Conflict($"Парсер уже в состоянии {State.Value}.");
		State = SubscribedParserState.Sleeping;
		Schedule = Schedule.WithStartedAt(DateTime.UtcNow);
		return Unit.Value;
	}

	public Result<Unit> FinishWork()
	{
		if (!State.IsWorking())
		{
			return Error.Conflict(
				$"Парсер не в состоянии {SubscribedParserState.Working.Value}, чтобы завершить работу."
			);
		}

		State = SubscribedParserState.Sleeping;
		Result<SubscribedParserSchedule> updated = Schedule.WithFinishedAt(DateTime.UtcNow);
		if (updated.IsFailure)
			return updated.Error;
		Schedule = updated.Value;
		return Unit.Value;
	}

	public static async Task<Result<SubscribedParser>> CreateNew(
		SubscribedParserId id,
		SubscribedParserIdentity identity,
		ISubscribedParsersRepository repository,
		CancellationToken ct = default
	)
	{
		if (await repository.Exists(identity, ct: ct))
		{
			return Error.Conflict(
				$"Парсер для домена {identity.DomainName} и типа {identity.ServiceType} уже существует."
			);
		}

		ParsingStatistics statistics = ParsingStatistics.New();
		SubscribedParserState state = SubscribedParserState.Disabled;
		SubscribedParserSchedule schedule = SubscribedParserSchedule.New();
		SubscribedParser parser = new(id, identity, statistics, state, schedule);
		await repository.Add(parser, ct: ct);
		return parser;
	}

	public static SubscribedParser CreateCopy(SubscribedParser parser) => new(parser);

	private bool BelongsToParser(SubscribedParserLink link) => link.ParserId == Id;

	private bool ContainsLinkWithName(SubscribedParserLink link) =>
		Links.Any(l => l.UrlInfo.Name == link.UrlInfo.Name && l.Id != link.Id);

	private void AddLinkToCollection(SubscribedParserLink link) => Links = [.. Links, link];

	private bool ContainsLinkWithUrl(SubscribedParserLink link) =>
		Links.Any(l => l.UrlInfo.Url == link.UrlInfo.Url && l.Id != link.Id);

	private bool AllLinksHaveUniqueName(out string[] duplicates)
	{
		duplicates = [.. Links.GroupBy(l => l.UrlInfo.Name).Where(g => g.Count() > 1).Select(g => g.Key)];
		return duplicates.Length == 0;
	}

	private bool AllLinksHaveUniqueUrl(out string[] duplicates)
	{
		duplicates = [.. Links.GroupBy(l => l.UrlInfo.Url).Where(g => g.Count() > 1).Select(g => g.Key)];
		return duplicates.Length == 0;
	}

	private Result<int> GetSpecifiedWaitDays()
	{
		return !Schedule.WaitDays.HasValue
			? (Result<int>)Error.Conflict("Дни ожидания не указаны.")
			: Result.Success(Schedule.WaitDays.Value);
	}

	private bool HasNoLinks() => Links.Count == 0;

	private bool AllLinksAreInactive() => Links.All(l => !l.Active);
}
