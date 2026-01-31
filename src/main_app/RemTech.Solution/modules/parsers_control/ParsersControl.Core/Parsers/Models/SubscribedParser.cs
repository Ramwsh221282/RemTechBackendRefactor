using ParsersControl.Core.Common;
using ParsersControl.Core.Contracts;
using ParsersControl.Core.Features.UpdateParserLinks;
using ParsersControl.Core.ParserLinks.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Models;

/// <summary>
/// Подписанный парсер.
/// </summary>
public sealed class SubscribedParser
{
	/// <summary>
	/// Создаёт подписанный парсер с ссылками на основе другого парсера.
	/// </summary>
	/// <param name="parser">Исходный подписанный парсер.</param>
	/// <param name="links">Ссылки для нового подписанного парсера.</param>
	public SubscribedParser(SubscribedParser parser, IEnumerable<SubscribedParserLink> links)
		: this(parser.Id, parser.Identity, parser.Statistics, parser.State, parser.Schedule, [.. links]) { }

	/// <summary>
	/// Создаёт подписанный парсер с ссылками.
	/// </summary>
	/// <param name="id">Идентификатор подписанного парсера.</param>
	/// <param name="identity">Идентичность (сервис) парсера.</param>
	/// <param name="statistics">Статистика парсера.</param>
	/// <param name="state">Состояние парсера.</param>
	/// <param name="schedule">Расписание парсера.</param>
	/// <param name="links">Ссылки парсера.</param>
	public SubscribedParser(
		SubscribedParserId id,
		SubscribedParserIdentity identity,
		ParsingStatistics statistics,
		SubscribedParserState state,
		SubscribedParserSchedule schedule,
		IReadOnlyList<SubscribedParserLink> links
	)
		: this(id, identity, statistics, state, schedule)
	{
		Links = [.. links];
	}

	/// <summary>
	/// Создаёт подписанный парсер.
	/// </summary>
	/// <param name="id">Идентификатор подписанного парсера.</param>
	/// <param name="identity">Идентичность (сервис) парсера.</param>
	/// <param name="statistics">Статистика парсера.</param>
	/// <param name="state">Состояние парсера.</param>
	/// <param name="schedule">Расписание парсера.</param>
	public SubscribedParser(
		SubscribedParserId id,
		SubscribedParserIdentity identity,
		ParsingStatistics statistics,
		SubscribedParserState state,
		SubscribedParserSchedule schedule
	)
	{
		Id = id;
		Identity = identity;
		Statistics = statistics;
		State = state;
		Schedule = schedule;
		Links = [];
	}

	private SubscribedParser(SubscribedParser parser)
		: this(
			parser.Id,
			parser.Identity,
			parser.Statistics,
			parser.State,
			parser.Schedule,
			[.. parser.Links.Select(SubscribedParserLink.CreateCopy)]
		) { }

	/// <summary>
	/// Ссылки парсера.
	/// </summary>
	public IReadOnlyList<SubscribedParserLink> Links { get; private set; }

	/// <summary>
	/// Идентификатор подписанного парсера.
	/// </summary>
	public SubscribedParserId Id { get; }

	/// <summary>
	/// Идентичность (сервис) парсера.
	/// </summary>
	public SubscribedParserIdentity Identity { get; }

	/// <summary>
	/// Статистика парсера.
	/// </summary>
	public ParsingStatistics Statistics { get; private set; }

	/// <summary>
	/// Состояние парсера.
	/// </summary>
	public SubscribedParserState State { get; private set; }

	/// <summary>
	/// Расписание парсера.
	/// </summary>
	public SubscribedParserSchedule Schedule { get; private set; }

	/// <summary>
	/// Создаёт новый подписанный парсер.
	/// </summary>
	/// <param name="id">Идентификатор подписанного парсера.</param>
	/// <param name="identity">Идентичность (сервис) парсера.</param>
	/// <param name="repository">Репозиторий подписанных парсеров.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Результат создания подписанного парсера.</returns>
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

	/// <summary>
	/// Создаёт копию подписанного парсера.
	/// </summary>
	/// <param name="parser">Подписанный парсер для копирования.</param>
	/// <returns>Копия подписанного парсера.</returns>
	public static SubscribedParser CreateCopy(SubscribedParser parser)
	{
		return new(parser);
	}

	/// <summary>
	/// Добавляет несколько ссылок к парсеру.
	/// </summary>
	/// <param name="urlInfos">Информация о ссылках для добавления.</param>
	/// <returns>Результат добавления ссылок.</returns>
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
			{
				return Error.Conflict($"Парсер уже содержит ссылку с именем {link.UrlInfo.Name}.");
			}
			if (ContainsLinkWithUrl(link))
			{
				return Error.Conflict($"Парсер уже содержит ссылку с адресом {link.UrlInfo.Url}.");
			}

			newLinks.Add(link);
		}

		Links = [.. Links, .. newLinks];
		return newLinks;
	}

	/// <summary>
	/// Добавляет количество обработанных данных к статистике парсера.
	/// </summary>
	/// <param name="amount">Количество обработанных данных для добавления.</param>
	/// <returns>Результат добавления количества обработанных данных.</returns>
	public Result<Unit> AddParserAmount(int amount)
	{
		Result<ParsingStatistics> updated = Statistics.IncreaseParsedCount(amount);
		if (updated.IsFailure)
		{
			return updated.Error;
		}

		Statistics = updated.Value;
		return Unit.Value;
	}

	/// <summary>
	/// Добавляет время работы к статистике парсера.
	/// </summary>
	/// <param name="totalElapsedSeconds">Общее количество секунд работы для добавления.</param>
	/// <returns>Результат добавления времени работы.</returns>
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
		{
			return updated.Error;
		}

		Statistics = updated.Value;
		return Unit.Value;
	}

	/// <summary>
	/// Добавляет ссылку к парсеру.
	/// </summary>
	/// <param name="urlInfo">Информация о ссылке для добавления.</param>
	/// <returns>Результат добавления ссылки.</returns>
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
		{
			return Error.Conflict($"Парсер уже содержит ссылку с именем {link.UrlInfo.Name}.");
		}
		if (ContainsLinkWithUrl(link))
		{
			return Error.Conflict($"Парсер уже содержит ссылку с адресом {link.UrlInfo.Url}.");
		}

		AddLinkToCollection(link);
		return link;
	}

	/// <summary>
	/// Добавляет ссылку к парсеру, игнорируя политику состояния.
	/// </summary>
	/// <param name="link">Ссылка для добавления.</param>
	/// <returns>Результат добавления ссылки.</returns>
	public Result<Unit> AddLinkIgnoringStatePolitics(SubscribedParserLink link)
	{
		if (ContainsLinkWithUrl(link))
		{
			return Error.Conflict($"Парсер уже содержит ссылку с адресом {link.UrlInfo.Url}.");
		}
		AddLinkToCollection(link);
		return Result.Success(Unit.Value);
	}

	/// <summary>
	/// Сбрасывает время работы в статистике парсера.
	/// </summary>
	public void ResetWorkTime()
	{
		Statistics = Statistics.ResetWorkTime();
	}

	/// <summary>
	/// Сбрасывает количество обработанных данных в статистике парсера.
	/// </summary>
	public void ResetParsedCount()
	{
		Statistics = Statistics.ResetParsedCount();
	}

	/// <summary>
	/// 	Начинает ожидание парсера.
	/// </summary>
	/// <returns>Результат начала ожидания парсера.</returns>
	public Result<Unit> StartWaiting()
	{
		if (State.IsWorking()) { }
		if (HasNoLinks())
		{
			return Error.Conflict("Парсер не содержит ссылок.");
		}

		if (AllLinksAreInactive())
		{
			return Error.Conflict("Парсер не содержит активных ссылок.");
		}
		Result<int> waitDays = GetSpecifiedWaitDays();
		if (waitDays.IsFailure)
		{
			return waitDays.Error;
		}

		State = SubscribedParserState.Sleeping;
		Schedule = Schedule.WithNextRun(DateTime.UtcNow.AddDays(waitDays.Value));
		return Unit.Value;
	}

	/// <summary>
	/// Начинает работу парсера.
	/// </summary>
	/// <returns>Результат начала работы парсера.</returns>
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
			(_, _, true, _) => () => Error.Conflict("Парсер не содержит ссылок."),
			(_, _, _, true) => () => Error.Conflict("Парсер не содержит активных ссылок."),
			(_, true, _, _) => () => Error.Conflict($"Парсер в состоянии {State.Value}. Невозможно начать работу."),
			(true, _, _, _) => () => Error.Conflict($"Парсер уже в состоянии {State.Value}."),
		};

		return operation();
	}

	/// <summary>
	/// Изменяет дни ожидания в расписании парсера.
	/// </summary>
	/// <param name="waitDays">Количество дней ожидания.</param>
	/// <returns>Результат изменения дней ожидания.</returns>
	public Result<Unit> ChangeScheduleWaitDays(int waitDays)
	{
		if (State.IsWorking())
		{
			return Error.Conflict($"Парсер в состоянии {State.Value}. Невозможно изменить дни ожидания.");
		}

		Result<SubscribedParserSchedule> updated = Schedule.WithWaitDays(waitDays);
		if (updated.IsFailure)
		{
			return updated.Error;
		}

		Schedule = updated.Value;
		return Unit.Value;
	}

	/// <summary>
	/// Изменяет дату следующего запуска в расписании парсера.
	/// </summary>
	/// <param name="nextRun">Дата следующего запуска.</param>
	/// <returns>Результат изменения даты следующего запуска.</returns>
	public Result<Unit> ChangeScheduleNextRun(DateTime nextRun)
	{
		if (State.IsWorking())
		{
			return Error.Conflict($"Парсер в состоянии {State.Value}. Невозможно изменить дату следующего запуска.");
		}

		Result<SubscribedParserSchedule> updated = Schedule.WithNextRun(nextRun);
		if (updated.IsFailure)
		{
			return updated.Error;
		}

		Schedule = updated.Value;
		return Unit.Value;
	}

	/// <summary>
	/// Находит ссылку по предикату.
	/// </summary>
	/// <param name="predicate">Предикат для поиска ссылки.</param>
	/// <returns>Результат поиска ссылки.</returns>
	public Result<SubscribedParserLink> FindLink(Func<SubscribedParserLinkUrlInfo, bool> predicate)
	{
		SubscribedParserLink? link = Links.FirstOrDefault(l => predicate(l.UrlInfo));
		if (link is null)
		{
			return Error.NotFound("Ссылка не найдена.");
		}

		return link;
	}

	/// <summary>
	/// Находит ссылку по идентификатору.
	/// </summary>
	/// <param name="id">Идентификатор ссылки.</param>
	/// <returns>Результат поиска ссылки.</returns>
	public Result<SubscribedParserLink> FindLink(Guid id)
	{
		SubscribedParserLink? link = Links.FirstOrDefault(l => l.Id.Value == id);
		return link is null
			? Result.Failure<SubscribedParserLink>(Error.NotFound("Ссылка не найдена."))
			: Result.Success(link);
	}

	/// <summary>
	/// Находит ссылку по идентификатору.
	/// </summary>
	/// <param name="id">Идентификатор ссылки.</param>
	/// <returns>Результат поиска ссылки.</returns>
	public Result<SubscribedParserLink> FindLink(SubscribedParserLinkId id)
	{
		SubscribedParserLink? link = Links.FirstOrDefault(l => l.Id == id);
		return link is null
			? Result.Failure<SubscribedParserLink>(Error.NotFound("Ссылка не найдена."))
			: Result.Success(link);
	}

	/// <summary>
	/// Изменяет активность ссылки парсера.
	/// </summary>
	/// <param name="link">Ссылка парсера.</param>
	/// <param name="isActive">Флаг активности ссылки.</param>
	/// <returns>Результат изменения активности ссылки.</returns>
	public Result<SubscribedParserLink> ChangeLinkActivity(SubscribedParserLink link, bool isActive)
	{
		if (State.IsWorking())
		{
			return Error.Conflict($"Парсер в состоянии {State.Value}. Невозможно изменить активность ссылки.");
		}
		if (!BelongsToParser(link))
		{
			return Error.Conflict($"Ссылка {link.Id.Value} не принадлежит парсеру {Id.Value}.");
		}
		if (isActive)
		{
			link.Enable();
		}
		else
		{
			link.Disable();
		}
		return link;
	}

	/// <summary>
	/// Отключает парсер.
	/// </summary>
	public void Disable()
	{
		State = SubscribedParserState.Disabled;
		Schedule = Schedule.WithFinishedAt(DateTime.UtcNow);
	}

	/// <summary>
	/// Обновляет ссылки парсера.
	/// </summary>
	/// <param name="updater">Обновляющие данные для ссылок парсера.</param>
	/// <returns>Результат обновления ссылок.</returns>
	public Result<IEnumerable<SubscribedParserLink>> UpdateLinks(IEnumerable<ParserLinkUpdater> updater)
	{
		if (State.IsWorking())
		{
			return Error.Conflict($"Парсер в состоянии {State.Value}. Невозможно изменить ссылки.");
		}

		foreach (SubscribedParserLink link in Links)
		{
			ParserLinkUpdater? linkUpdater = updater.FirstOrDefault(u => u.UpdateBelongsTo(link));
			if (linkUpdater is null)
			{
				continue;
			}

			Result update = linkUpdater.Update(link);
			if (update.IsFailure)
			{
				return update.Error;
			}
		}

		if (!AllLinksHaveUniqueName(out string[] dupNames))
		{
			return Error.Conflict("Парсер содержит ссылки с одинаковым именем: " + string.Join(", ", dupNames));
		}

		return !AllLinksHaveUniqueUrl(out string[] dupUrls)
			? Error.Conflict("Парсер содержит ссылки с одинаковым адресом: " + string.Join(", ", dupUrls))
			: Result.Success(Links.AsEnumerable());
	}

	/// <summary>
	/// Завершает работу парсера с указанием времени работы.
	/// </summary>
	/// <param name="totalElapsedSeconds">Общее время работы парсера в секундах.</param>
	/// <returns>Результат завершения работы парсера.</returns>
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
		{
			return update.Error;
		}

		Statistics = update.Value;
		State = SubscribedParserState.Sleeping;
		Schedule = Schedule.WithFinishedAt(DateTime.UtcNow);
		return Unit.Value;
	}

	/// <summary>
	/// Навсегда включает парсер.
	/// </summary>
	/// <returns>Результат включения парсера.</returns>
	public Result<Unit> PermantlyEnable()
	{
		if (HasNoLinks())
		{
			return Error.Conflict("Парсер не содержит ссылок.");
		}

		if (State.IsWorking())
		{
			return Error.Conflict($"Парсер уже в состоянии {State.Value}.");
		}

		State = SubscribedParserState.Working;
		Schedule = Schedule.WithStartedAt(DateTime.UtcNow);
		return Unit.Value;
	}

	/// <summary>
	/// Навсегда отключает парсер.
	/// </summary>
	public void PermanentlyDisable()
	{
		State = SubscribedParserState.Disabled;
		Schedule = Schedule.WithFinishedAt(DateTime.UtcNow);
	}

	/// <summary>
	/// Удаляет ссылку из парсера.
	/// </summary>
	/// <param name="link">Ссылка для удаления.</param>
	/// <returns>Результат удаления ссылки.</returns>
	public Result<SubscribedParserLink> RemoveLink(SubscribedParserLink link)
	{
		if (!State.IsDisabled() && !State.IsSleeping())
		{
			return Error.Conflict(
				$"Для удаления ссылки парсер должен быть в состоянии {SubscribedParserState.Disabled.Value} или {SubscribedParserState.Sleeping.Value}."
			);
		}

		if (!BelongsToParser(link))
		{
			return Error.Conflict($"Ссылка {link.Id.Value} не принадлежит парсеру {Id.Value}.");
		}

		Links = [.. Links.Where(l => l.Id != link.Id)];
		return link;
	}

	/// <summary>
	/// Добавляет количество обработанных данных к ссылке парсера.
	/// </summary>
	/// <param name="link">Ссылка парсера.</param>
	/// <param name="count">Количество обработанных данных для добавления.</param>
	/// <returns>Результат добавления количества обработанных данных.</returns>
	public Result<SubscribedParserLink> AddLinkParsedAmount(SubscribedParserLink link, int count)
	{
		if (!State.IsWorking())
		{
			return Result.Failure<SubscribedParserLink>(
				Error.Conflict(
					$"Для добавления количества обработанных данных парсер должен быть в состоянии {SubscribedParserState.Working.Value}."
				)
			);
		}

		if (!BelongsToParser(link))
		{
			return Result.Failure<SubscribedParserLink>(
				Error.Conflict($"Ссылка {link.Id.Value} не принадлежит парсеру {Id.Value}.")
			);
		}

		Result result = link.AddParsedCount(count);
		return result.IsFailure ? Result.Failure<SubscribedParserLink>(result.Error) : Result.Success(link);
	}

	/// <summary>
	/// Добавляет время работы к ссылке парсера.
	/// </summary>
	/// <param name="link">Ссылка парсера.</param>
	/// <param name="totalElapsedSeconds">Время работы для добавления в секундах.</param>
	/// <returns>Результат добавления времени работы.</returns>
	public Result<SubscribedParserLink> AddLinkWorkTime(SubscribedParserLink link, long totalElapsedSeconds)
	{
		if (!State.IsWorking())
		{
			return Result.Failure<SubscribedParserLink>(
				Error.Conflict(
					$"Для добавления времени работы парсер должен быть в состоянии {SubscribedParserState.Working.Value}."
				)
			);
		}

		if (!BelongsToParser(link))
		{
			return Result.Failure<SubscribedParserLink>(
				Error.Conflict($"Ссылка {link.Id.Value} не принадлежит парсеру {Id.Value}.")
			);
		}

		Result result = link.AddWorkTime(totalElapsedSeconds);
		return result.IsFailure ? Result.Failure<SubscribedParserLink>(result.Error) : Result.Success(link);
	}

	/// <summary>
	/// Редактирует ссылку парсера.
	/// </summary>
	/// <param name="link">Ссылка парсера.</param>
	/// <param name="newName">Новое имя ссылки.</param>
	/// <param name="newUrl">Новый URL ссылки.</param>
	/// <returns>Результат редактирования ссылки.</returns>
	public Result<SubscribedParserLink> EditLink(SubscribedParserLink link, string? newName, string? newUrl)
	{
		if (!BelongsToParser(link))
		{
			return Error.Conflict($"Ссылка {link.Id.Value} не принадлежит парсеру {Id.Value}.");
		}

		Result<SubscribedParserLink> editResult = link.Edit(newName, newUrl);
		if (editResult.IsFailure)
		{
			return Result.Failure<SubscribedParserLink>(editResult.Error);
		}

		if (ContainsLinkWithName(editResult.Value))
		{
			return Error.Conflict($"Парсер уже содержит ссылку с именем {editResult.Value.UrlInfo.Name}.");
		}

		return ContainsLinkWithUrl(editResult.Value)
			? Error.Conflict($"Парсер уже содержит ссылку с адресом {editResult.Value.UrlInfo.Url}.")
			: editResult;
	}

	/// <summary>
	/// Навсегда включает парсер.
	/// </summary>
	/// <returns>Результат включения парсера.</returns>
	public Result<Unit> Enable()
	{
		if (State.IsSleeping())
		{
			return Error.Conflict($"Парсер уже в состоянии {State.Value}.");
		}

		State = SubscribedParserState.Working;
		Schedule = Schedule.WithStartedAt(DateTime.UtcNow);
		return Unit.Value;
	}

	/// <summary>
	/// Навсегда отключает парсер.
	/// </summary>
	/// <returns>Результат отключения парсера.</returns>
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
		{
			return updated.Error;
		}

		Schedule = updated.Value;
		return Unit.Value;
	}

	private bool BelongsToParser(SubscribedParserLink link)
	{
		return link.ParserId == Id;
	}

	private bool ContainsLinkWithName(SubscribedParserLink link)
	{
		return Links.Any(l => l.UrlInfo.Name == link.UrlInfo.Name && l.Id != link.Id);
	}

	private void AddLinkToCollection(SubscribedParserLink link)
	{
		Links = [.. Links, link];
	}

	private bool ContainsLinkWithUrl(SubscribedParserLink link)
	{
		return Links.Any(l => l.UrlInfo.Url == link.UrlInfo.Url && l.Id != link.Id);
	}

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

	private bool HasNoLinks()
	{
		return Links.Count == 0;
	}

	private bool AllLinksAreInactive()
	{
		return Links.All(l => !l.Active);
	}
}
