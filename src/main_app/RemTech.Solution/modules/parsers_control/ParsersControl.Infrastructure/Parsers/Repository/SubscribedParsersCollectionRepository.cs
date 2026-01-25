using System.Data;
using System.Data.Common;
using Dapper;
using Npgsql;
using ParsersControl.Core.Common;
using ParsersControl.Core.Contracts;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;

namespace ParsersControl.Infrastructure.Parsers.Repository;

public sealed class SubscribedParsersCollectionRepository(NpgSqlSession session, Serilog.ILogger logger)
	: ISubscribedParsersCollectionRepository
{
	private readonly SubscribedParsersCollectionChangeTracker _changeTracker = new(logger);

	public async Task<SubscribedParsersCollection> Get(
		SubscribedParsersCollectionQuery query,
		CancellationToken ct = default
	)
	{
		(DynamicParameters parameters, string filterSql) = FilterClause(query);
		string sql = $"""
			SELECT
			p.id as parser_id,
			p.type as parser_type,
			p.domain as parser_domain,
			p.state as parser_state,
			p.processed as parser_processed,
			p.elapsed_seconds as parser_elapsed_seconds,
			p.started_at as parser_started_at,
			p.finished_at as parser_finished_at,
			p.next_run as parser_next_run,
			p.wait_days as parser_wait_days,
			pl.id as link_id,
			pl.parser_id as link_parser_id,
			pl.name as link_name,
			pl.url as link_url,
			pl.elapsed_seconds as link_elapsed_seconds,
			pl.processed as link_processed,
			pl.is_active as link_is_active
			FROM parsers_control_module.registered_parsers p
			LEFT JOIN parsers_control_module.parser_links pl ON p.id = pl.parser_id
			{filterSql}
			""";
		CommandDefinition command = session.FormCommand(sql, parameters, ct);
		NpgsqlConnection connection = await session.GetConnection(ct);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		SubscribedParsersCollection collection = await MapFromReader(reader, ct);
		if (collection.IsEmpty())
			return collection;
		if (query.WithLock)
			await BlockParsers(collection.GetIdentifiers(), ct);
		_changeTracker.StartTracking(collection);
		return collection;
	}

	public Task<Result<Unit>> SaveChanges(SubscribedParsersCollection collection, CancellationToken ct = default) =>
		_changeTracker.SaveChanges(collection, session, ct);

	private static async Task<SubscribedParsersCollection> MapFromReader(DbDataReader reader, CancellationToken ct)
	{
		Dictionary<Guid, SubscribedParser> values = [];
		while (await reader.ReadAsync(ct))
		{
			Guid parserId = reader.GetValue<Guid>("parser_id");
			if (!values.TryGetValue(parserId, out SubscribedParser? parser))
			{
				parser = MapParser(parserId, reader);
				values.Add(parserId, parser);
			}

			Guid? linkId = reader.GetNullable<Guid>("link_id");
			if (!linkId.HasValue)
				continue;

			SubscribedParserLink link = MapParserLink(linkId.Value, parser, reader);
			Result<Unit> result = parser.AddLinkIgnoringStatePolitics(link);
			if (result.IsFailure)
			{
				throw new InvalidOperationException(
					$"Error at mapping parser link in {nameof(MapFromReader)} method in {nameof(SubscribedParsersCollectionRepository)}: "
						+ result.Error.Message
				);
			}
		}

		return new SubscribedParsersCollection(values.Select(p => p.Value));
	}

	private static SubscribedParserLink MapParserLink(Guid id, SubscribedParser parser, IDataReader reader)
	{
		string name = reader.GetValue<string>("link_name");
		string url = reader.GetValue<string>("link_url");
		int processed = reader.GetValue<int>("link_processed");
		long elapsedSeconds = reader.GetValue<long>("link_elapsed_seconds");
		bool active = reader.GetValue<bool>("link_is_active");
		return SubscribedParserLink.Create(
			parserId: parser.Id,
			id: SubscribedParserLinkId.From(id),
			urlInfo: SubscribedParserLinkUrlInfo.Create(url, name),
			statistics: new ParsingStatistics(
				workTime: ParsingWorkTime.FromTotalElapsedSeconds(elapsedSeconds),
				parsedCount: ParsedCount.Create(processed)
			),
			active: active
		);
	}

	private static SubscribedParser MapParser(Guid id, IDataReader reader)
	{
		string type = reader.GetValue<string>("parser_type");
		string domain = reader.GetValue<string>("parser_domain");
		string state = reader.GetValue<string>("parser_state");
		int processed = reader.GetValue<int>("parser_processed");
		int elapsedSeconds = reader.GetValue<int>("parser_elapsed_seconds");
		DateTime? startedAt = reader.GetNullable<DateTime>("parser_started_at");
		DateTime? finishedAt = reader.GetNullable<DateTime>("parser_finished_at");
		DateTime? nextRun = reader.GetNullable<DateTime>("parser_next_run");
		int? waitDays = reader.GetNullable<int>("parser_wait_days");
		return new SubscribedParser(
			id: SubscribedParserId.Create(id),
			identity: SubscribedParserIdentity.Create(domainName: domain, serviceType: type),
			statistics: new ParsingStatistics(
				workTime: ParsingWorkTime.FromTotalElapsedSeconds(elapsedSeconds),
				parsedCount: ParsedCount.Create(processed)
			),
			state: SubscribedParserState.FromString(state),
			schedule: SubscribedParserSchedule.Create(startedAt, finishedAt, nextRun, waitDays)
		);
	}

	private static (DynamicParameters Parameters, string FilterSql) FilterClause(SubscribedParsersCollectionQuery query)
	{
		DynamicParameters parameters = new();
		List<string> filters = [];

		if (query.Identifiers is not null && query.Identifiers.Any())
		{
			filters.Add("p.id = ANY (@ids)");
			parameters.Add("@ids", query.Identifiers.ToArray());
		}

		if (query.Domains is not null && query.Domains.Any())
		{
			filters.Add("p.domain = ANY(@domains)");
			parameters.Add("@domains", query.Domains.ToArray());
		}

		if (query.Types is not null && query.Types.Any())
		{
			filters.Add("p.type = ANY(@types)");
			parameters.Add("@types", query.Types.ToArray());
		}

		return (parameters, filters.Count == 0 ? string.Empty : $" WHERE {string.Join(" AND ", filters)}");
	}

	private async Task BlockParsers(IEnumerable<Guid> identifiers, CancellationToken ct)
	{
		const string sql = """
			WITH blocked_parsers AS (
			SELECT p.id FROM parsers_control_module.registered_parsers p
			WHERE p.id IN (@ids)
			FOR UPDATE OF p
			)
			SELECT
			pl.id as link_id
			FROM parsers_control_module.parser_links pl
			JOIN blocked_parsers bp ON pl.parser_id = bp.id
			FOR UPDATE OF pl;
			""";
		DynamicParameters parameters = new();
		parameters.Add("@ids", identifiers.ToArray(), DbType.Guid);
		CommandDefinition command = new(sql, parameters, transaction: session.Transaction, cancellationToken: ct);
		NpgsqlConnection connection = await session.GetConnection(ct);
		await connection.ExecuteAsync(command);
	}

	private sealed class SubscribedParsersCollectionChangeTracker(Serilog.ILogger logger)
	{
		private readonly Dictionary<Guid, SubscribedParser> _parsers = [];
		private readonly Serilog.ILogger _logger = logger.ForContext<SubscribedParsersCollectionChangeTracker>();

		public void StartTracking(SubscribedParsersCollection collection)
		{
			foreach (SubscribedParser parser in collection.Read())
			{
				Guid id = parser.Id.Value;
				_parsers.TryAdd(id, SubscribedParser.CreateCopy(parser));
			}
		}

		public async Task<Result<Unit>> SaveChanges(
			SubscribedParsersCollection parsers,
			NpgSqlSession session,
			CancellationToken ct = default
		)
		{
			int index = 0;
			DynamicParameters parameters = new();
			List<string> ids = [];
			List<string> stateCases = [];
			List<string> waitDaysCases = [];
			List<string> nextRunCases = [];
			List<string> finishedAtCases = [];
			List<string> startedAtCases = [];
			List<string> processedCases = [];
			List<string> elapsedSecondsCases = [];
			foreach (SubscribedParser parser in parsers.Read())
			{
				if (!_parsers.TryGetValue(parser.Id.Value, out SubscribedParser? existing))
				{
					continue;
				}

				string idParam = $"@id_{index}";
				parameters.Add(idParam, parser.Id.Value, DbType.Guid);
				ids.Add(idParam);
				string whenClause = $"WHEN p.id = {idParam}";

				if (parser.State.Value != existing.State.Value)
				{
					string stateParam = $"@state_{index}";
					parameters.Add(stateParam, parser.State.Value, DbType.String);
					stateCases.Add($"{whenClause} THEN {stateParam}");
				}

				if (parser.Schedule.WaitDays != existing.Schedule.WaitDays)
				{
					string waitDaysParam = $"@wait_days_{index}";
					parameters.Add(
						waitDaysParam,
						parser.Schedule.WaitDays is null ? DBNull.Value : parser.Schedule.WaitDays.Value,
						DbType.Int32
					);
					waitDaysCases.Add($"{whenClause} THEN {waitDaysParam}");
				}

				if (parser.Schedule.NextRun != existing.Schedule.NextRun)
				{
					string nextRunParam = $"@next_run_{index}";
					parameters.Add(
						nextRunParam,
						parser.Schedule.NextRun is null ? DBNull.Value : parser.Schedule.NextRun.Value,
						DbType.DateTime
					);
					nextRunCases.Add($"{whenClause} THEN {nextRunParam}");
				}

				if (parser.Schedule.FinishedAt != existing.Schedule.FinishedAt)
				{
					string finishedAtParam = $"@finished_at_{index}";
					parameters.Add(
						finishedAtParam,
						parser.Schedule.FinishedAt is null ? DBNull.Value : parser.Schedule.FinishedAt.Value,
						DbType.DateTime
					);
					finishedAtCases.Add($"{whenClause} THEN {finishedAtParam}");
				}

				if (parser.Schedule.StartedAt != existing.Schedule.StartedAt)
				{
					string startedAtParam = $"@started_at_{index}";
					parameters.Add(
						startedAtParam,
						parser.Schedule.StartedAt is null ? DBNull.Value : parser.Schedule.StartedAt.Value,
						DbType.DateTime
					);
					startedAtCases.Add($"{whenClause} THEN {startedAtParam}");
				}

				if (parser.Statistics.ParsedCount.Value != existing.Statistics.ParsedCount.Value)
				{
					string processedParam = $"@processed_{index}";
					parameters.Add(processedParam, parser.Statistics.ParsedCount.Value, DbType.Int32);
					processedCases.Add($"{whenClause} THEN {processedParam}");
				}

				if (parser.Statistics.WorkTime.TotalElapsedSeconds != existing.Statistics.WorkTime.TotalElapsedSeconds)
				{
					string elapsedSecondsParam = $"@elapsed_seconds_{index}";
					parameters.Add(elapsedSecondsParam, parser.Statistics.WorkTime.TotalElapsedSeconds, DbType.Int64);
					elapsedSecondsCases.Add($"{whenClause} THEN {elapsedSecondsParam}");
				}

				index++;
			}

			List<string> setClauses = [];

			if (ids.Count == 0)
				return Result.Success(Unit.Value);
			if (stateCases.Count > 0)
				setClauses.Add($"state = CASE {string.Join(" ", stateCases)} ELSE state END");
			if (waitDaysCases.Count > 0)
				setClauses.Add($"wait_days = CASE {string.Join(" ", waitDaysCases)} ELSE wait_days END");
			if (nextRunCases.Count > 0)
				setClauses.Add($"next_run = CASE {string.Join(" ", nextRunCases)} ELSE next_run END");
			if (finishedAtCases.Count > 0)
				setClauses.Add($"finished_at = CASE {string.Join(" ", finishedAtCases)} ELSE finished_at END");
			if (startedAtCases.Count > 0)
				setClauses.Add($"started_at = CASE {string.Join(" ", startedAtCases)} ELSE started_at END");
			if (processedCases.Count > 0)
				setClauses.Add($"processed = CASE {string.Join(" ", processedCases)} ELSE processed END");
			if (elapsedSecondsCases.Count > 0)
			{
				setClauses.Add(
					$"elapsed_seconds = CASE {string.Join(" ", elapsedSecondsCases)} ELSE elapsed_seconds END"
				);
			}

			if (setClauses.Count == 0)
				return Result.Success(Unit.Value);

			string sql = $"""
				UPDATE parsers_control_module.registered_parsers p
				SET {string.Join(", ", setClauses)}
				WHERE p.id IN ({string.Join(", ", ids) + "::uuid"})
				""";

			CommandDefinition command = session.FormCommand(sql, parameters, ct);
			try
			{
				await session.Execute(command);
			}
			catch (Exception e)
			{
				_logger.Fatal(e, "Не удалось сохранить изменения.");
				return Error.Conflict("Не удалось сохранить изменения.");
			}

			return Result.Success(Unit.Value);
		}
	}
}
