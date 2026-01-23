using System.Data;
using Dapper;
using Npgsql;
using ParsersControl.Core.Common;
using ParsersControl.Core.Contracts;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;

namespace ParsersControl.Infrastructure.Parsers.Repository;

public sealed class SubscribedParsersRepository(NpgSqlSession session) : ISubscribedParsersRepository
{
	private readonly ChangeTracker _tracker = new(session);

	public Task<bool> Exists(SubscribedParserIdentity identity, CancellationToken ct = default)
	{
		const string sql = """
			SELECT EXISTS 
			    (SELECT 1 FROM parsers_control_module.registered_parsers
			              WHERE domain = @domain AND type = @type)
			""";
		object parameters = new { domain = identity.DomainName, type = identity.ServiceType };
		CommandDefinition command = new(sql, parameters, transaction: session.Transaction, cancellationToken: ct);
		return session.QuerySingleRow<bool>(command);
	}

	public Task Add(SubscribedParser parser, CancellationToken ct = default)
	{
		const string sql = """
			INSERT INTO parsers_control_module.registered_parsers
			    (id, type, domain, state, processed, elapsed_seconds, started_at, finished_at, wait_days, next_run)
			VALUES
			    (@id, @type, @domain, @state, @processed, @elapsed_seconds, @started_at, @finished_at, @wait_days, @next_run)
			""";
		object parameters = new
		{
			id = parser.Id.Value,
			type = parser.Identity.ServiceType,
			domain = parser.Identity.DomainName,
			state = parser.State.Value,
			processed = parser.Statistics.ParsedCount.Value,
			elapsed_seconds = parser.Statistics.WorkTime.TotalElapsedSeconds,
			started_at = parser.Schedule.StartedAt == null ? (object?)null : parser.Schedule.StartedAt.Value,
			finished_at = parser.Schedule.FinishedAt == null ? (object?)null : parser.Schedule.FinishedAt.Value,
			wait_days = parser.Schedule.WaitDays == null ? (object?)null : parser.Schedule.WaitDays.Value,
			next_run = parser.Schedule.NextRun == null ? (object?)null : parser.Schedule.NextRun.Value,
		};
		CommandDefinition command = new(sql, parameters, transaction: session.Transaction, cancellationToken: ct);
		return session.Execute(command);
	}

	public async Task Save(IEnumerable<SubscribedParser> parsers, CancellationToken ct = default) =>
		await _tracker.SaveChanges(parsers, ct);

	public async Task Save(SubscribedParser parser, CancellationToken ct = default) =>
		await _tracker.SaveChanges([parser], ct);

	public async Task<Result<SubscribedParser>> Get(SubscribedParserQuery query, CancellationToken ct = default)
	{
		(DynamicParameters parameters, string filterSql) = WhereClause(query);
		if (query.WithLock)
			await Block(query, ct);
		string sql = $"""
			SELECT p.id as parser_id, 
			       p.type as parser_type, 
			       p.domain as parser_domain, 
			       p.processed as parser_processed, 
			       p.elapsed_seconds as parser_elapsed_seconds, 
			       p.state as parser_state,
			       p.started_at as parser_started_at,
			       p.finished_at as parser_finished_at,
			       p.wait_days as parser_wait_days,
			       p.next_run as parser_next_run,
			       l.id as link_id,
			       l.parser_id as link_parser_id,
			       l.name as link_name,
			       l.url as link_url,
			       l.is_active as link_is_active,
			       l.processed as link_processed,
			       l.elapsed_seconds as link_elapsed_seconds
			FROM parsers_control_module.registered_parsers p
			LEFT JOIN parsers_control_module.parser_links l
			    ON p.id = l.parser_id
			{filterSql}
			""";

		CommandDefinition command = new(sql, parameters, transaction: session.Transaction, cancellationToken: ct);
		(SubscribedParser? parser, List<SubscribedParserLink> links) = await session.QuerySingleUsingReader(
			command,
			MapFromReader,
			MapLinkFromReader,
			new ParserIdComparer()
		);

		if (parser is null)
			return Error.NotFound("Парсер не найден.");
		SubscribedParser result = new(parser, links);
		_tracker.StartTracking(result);
		return result;
	}

	private sealed class ParserIdComparer : IEqualityComparer<SubscribedParser>
	{
		public bool Equals(SubscribedParser? x, SubscribedParser? y)
		{
			if (x is null || y is null)
				return x == y;
			return x.Id == y.Id;
		}

		public int GetHashCode(SubscribedParser obj) => obj.Id.GetHashCode();
	}

	private static SubscribedParserLink? MapLinkFromReader(IDataReader reader)
	{
		if (reader.IsNull("link_id"))
			return null;

		Guid id = reader.GetValue<Guid>("link_id");
		Guid parserId = reader.GetValue<Guid>("link_parser_id");
		string name = reader.GetValue<string>("link_name");
		string url = reader.GetValue<string>("link_url");
		int processed = reader.GetValue<int>("link_processed");
		long elapsedSeconds = reader.GetValue<long>("link_elapsed_seconds");
		bool active = reader.GetValue<bool>("link_is_active");

		SubscribedParserLinkId linkId = SubscribedParserLinkId.From(id);
		SubscribedParserId parserIdVo = SubscribedParserId.Create(parserId);
		SubscribedParserLinkUrlInfo urlInfo = SubscribedParserLinkUrlInfo.Create(url, name);
		ParsingWorkTime workTime = ParsingWorkTime.FromTotalElapsedSeconds(elapsedSeconds);
		ParsedCount parsedCount = ParsedCount.Create(processed);
		ParsingStatistics statistics = new(workTime, parsedCount);
		return SubscribedParserLink.Create(parserIdVo, linkId, urlInfo, statistics, active);
	}

	private static SubscribedParser MapFromReader(IDataReader reader)
	{
		Guid id = reader.GetValue<Guid>("parser_id");
		string type = reader.GetValue<string>("parser_type");
		string domain = reader.GetValue<string>("parser_domain");
		int processed = reader.GetValue<int>("parser_processed");
		long elapsedSeconds = reader.GetValue<long>("parser_elapsed_seconds");
		string state = reader.GetValue<string>("parser_state");
		DateTime? startedAt = reader.GetNullable<DateTime>("parser_started_at");
		DateTime? finishedAt = reader.GetNullable<DateTime>("parser_finished_at");
		DateTime? nextRun = reader.GetNullable<DateTime>("parser_next_run");
		int? waitDays = reader.GetNullable<int>("parser_wait_days");

		SubscribedParserId parserId = SubscribedParserId.Create(id);
		SubscribedParserIdentity identity = SubscribedParserIdentity.Create(domain, type);
		ParsingStatistics statistics = new(
			ParsingWorkTime.FromTotalElapsedSeconds(elapsedSeconds),
			ParsedCount.Create(processed)
		);
		SubscribedParserState parserState = SubscribedParserState.FromString(state);
		SubscribedParserSchedule schedule = SubscribedParserSchedule.Create(startedAt, finishedAt, nextRun, waitDays);

		return new SubscribedParser(parserId, identity, statistics, parserState, schedule);
	}

	private static (DynamicParameters parameters, string filterSql) WhereClause(SubscribedParserQuery query)
	{
		List<string> filterSql = [];
		DynamicParameters parameters = new();
		if (query.Id.HasValue)
		{
			filterSql.Add("p.id=@id");
			parameters.Add("@id", query.Id.Value, DbType.Guid);
		}

		if (!string.IsNullOrWhiteSpace(query.Domain))
		{
			filterSql.Add("p.domain=@domain");
			parameters.Add("@domain", query.Domain, DbType.String);
		}

		if (!string.IsNullOrWhiteSpace(query.Type))
		{
			filterSql.Add("p.type=@type");
			parameters.Add("@type", query.Type, DbType.String);
		}

		return filterSql.Count == 0 ? (parameters, "") : (parameters, $" WHERE {string.Join(" AND ", filterSql)}");
	}

	private async Task Block(SubscribedParserQuery query, CancellationToken ct)
	{
		(DynamicParameters parameters, string filterSql) = WhereClause(query);
		string sql = $"""
			SELECT p.id 
			FROM parsers_control_module.registered_parsers p
			{filterSql}
			FOR UPDATE OF p
			""";
		CommandDefinition command = new(sql, parameters, transaction: session.Transaction, cancellationToken: ct);
		await session.Execute(command);
	}

	private sealed class ChangeTracker(NpgSqlSession session)
	{
		private NpgSqlSession Session { get; } = session;
		private readonly Dictionary<Guid, SubscribedParser> _trackingParsers = [];

		public void StartTracking(SubscribedParser parser)
		{
			SubscribedParser copy = SubscribedParser.CreateCopy(parser);
			_trackingParsers.TryAdd(copy.Id.Value, copy);
		}

		public async Task SaveChanges(IEnumerable<SubscribedParser> parsers, CancellationToken ct = default)
		{
			await SaveParserChanges(parsers, ct);
			await SaveLinkChanges(parsers, ct);
		}

		private async Task SaveParserChanges(IEnumerable<SubscribedParser> parsers, CancellationToken ct = default)
		{
			DynamicParameters parameters = new();

			string updateClause = FormUpdateClauses(parsers, parameters);
			if (string.IsNullOrWhiteSpace(updateClause))
				return;

			CommandDefinition command = new(
				updateClause,
				parameters,
				transaction: Session.Transaction,
				cancellationToken: ct
			);

			NpgsqlConnection connection = await Session.GetConnection(ct);
			await connection.ExecuteAsync(command);
		}

		private async Task SaveLinkChanges(IEnumerable<SubscribedParser> parsers, CancellationToken ct = default)
		{
			foreach (SubscribedParser parser in GetTrackingParsers(parsers))
			{
				SubscribedParser original = _trackingParsers[parser.Id.Value];
				await RemoveLinks(GetLinksToRemove(original, parser), ct);
				await AddLinks(GetLinksToAdd(original, parser), ct);
				await UpdateLinks(GetLinksToUpdate(original, parser), GetOriginalLinksToCompare(original, parser), ct);
			}
		}

		private async Task UpdateLinks(
			IEnumerable<SubscribedParserLink> linksToUpdate,
			Dictionary<Guid, SubscribedParserLink> linksToCompare,
			CancellationToken ct
		)
		{
			SubscribedParserLink[] linksToUpdateArray = [.. linksToUpdate];
			if (linksToUpdateArray.Length == 0)
				return;
			if (linksToCompare.Count == 0)
				return;

			Guid[] ids = [.. linksToUpdateArray.Select(l => l.Id.Value)];
			List<string> setClauses = [];
			DynamicParameters parameters = new();

			if (linksToUpdateArray.Any(l => l.Active != linksToCompare[l.Id.Value].Active))
			{
				string caseWhen = string.Join(
					" ",
					linksToUpdateArray.Select(
						(l, i) =>
						{
							string paramName = $"@active_{i}";
							parameters.Add(paramName, l.Active, DbType.Boolean);
							return $"{CreateLinkWhenClause(i)} THEN {paramName}";
						}
					)
				);
				setClauses.Add($"is_active = CASE {caseWhen} ELSE is_active END");
			}

			if (
				linksToUpdateArray.Any(l =>
					l.Statistics.ParsedCount.Value != linksToCompare[l.Id.Value].Statistics.ParsedCount.Value
				)
			)
			{
				string caseWhen = string.Join(
					" ",
					linksToUpdateArray.Select(
						(l, i) =>
						{
							string paramName = $"@processed_{i}";
							parameters.Add(paramName, l.Statistics.ParsedCount.Value, DbType.Int32);
							return $"{CreateLinkWhenClause(i)} THEN {paramName}";
						}
					)
				);
				setClauses.Add($"processed = CASE {caseWhen} ELSE processed END");
			}

			if (
				linksToUpdateArray.Any(l =>
					l.Statistics.WorkTime.TotalElapsedSeconds
					!= linksToCompare[l.Id.Value].Statistics.WorkTime.TotalElapsedSeconds
				)
			)
			{
				string caseWhen = string.Join(
					" ",
					linksToUpdateArray.Select(
						(l, i) =>
						{
							string paramName = $"@elapsed_seconds_{i}";
							parameters.Add(paramName, l.Statistics.WorkTime.TotalElapsedSeconds, DbType.Int64);
							return $"{CreateLinkWhenClause(i)} THEN {paramName}";
						}
					)
				);
				setClauses.Add($"elapsed_seconds = CASE {caseWhen} ELSE elapsed_seconds END");
			}

			if (linksToUpdateArray.Any(l => l.UrlInfo.Name != linksToCompare[l.Id.Value].UrlInfo.Name))
			{
				string caseWhen = string.Join(
					" ",
					linksToUpdateArray.Select(
						(l, i) =>
						{
							string paramName = $"@name_{i}";
							parameters.Add(paramName, l.UrlInfo.Name, DbType.String);
							return $"{CreateLinkWhenClause(i)} THEN {paramName}";
						}
					)
				);
				setClauses.Add($"name = CASE {caseWhen} ELSE name END");
			}

			if (linksToUpdateArray.Any(l => l.UrlInfo.Url != linksToCompare[l.Id.Value].UrlInfo.Url))
			{
				string caseWhen = string.Join(
					" ",
					linksToUpdateArray.Select(
						(l, i) =>
						{
							string paramName = $"@url_{i}";
							parameters.Add(paramName, l.UrlInfo.Url, DbType.String);
							return $"{CreateLinkWhenClause(i)} THEN {paramName}";
						}
					)
				);
				setClauses.Add($"url = CASE {caseWhen} ELSE url END");
			}

			if (setClauses.Count == 0)
				return;

			for (int i = 0; i < ids.Length; i++)
			{
				Guid value = ids[i];
				string paramName = $"@id_{i}";
				parameters.Add(paramName, value, DbType.Guid);
			}

			parameters.Add("@ids", ids);

			string sql = $"""
				UPDATE parsers_control_module.parser_links pl
				SET {string.Join(", ", setClauses)}
				WHERE pl.id = ANY (@ids)
				""";

			CommandDefinition command = Session.FormCommand(sql, parameters, ct);
			await Session.Execute(command);
		}

		private static Dictionary<Guid, SubscribedParserLink> GetOriginalLinksToCompare(
			SubscribedParser original,
			SubscribedParser updated
		) =>
			original
				.Links.IntersectBy(updated.Links.Select(l => l.Id.Value), l => l.Id.Value)
				.ToDictionary(l => l.Id.Value, l => l);

		private static IEnumerable<SubscribedParserLink> GetLinksToUpdate(
			SubscribedParser original,
			SubscribedParser updated
		) => updated.Links.IntersectBy(original.Links.Select(l => l.Id.Value), l => l.Id.Value);

		private static IEnumerable<SubscribedParserLink> GetLinksToRemove(
			SubscribedParser original,
			SubscribedParser updated
		) => original.Links.ExceptBy(updated.Links.Select(l => l.Id.Value), l => l.Id.Value);

		private static IEnumerable<SubscribedParserLink> GetLinksToAdd(
			SubscribedParser original,
			SubscribedParser updated
		) => updated.Links.ExceptBy(original.Links.Select(l => l.Id.Value), l => l.Id.Value);

		private async Task RemoveLinks(IEnumerable<SubscribedParserLink> links, CancellationToken ct)
		{
			if (!links.Any())
				return;
			const string sql = "DELETE FROM parsers_control_module.parser_links pl WHERE pl.id = ANY (@ids)";
			Guid[] ids = [.. links.Select(l => l.Id.Value)];
			DynamicParameters parameters = new();
			parameters.Add("@ids", ids);
			CommandDefinition command = Session.FormCommand(sql, parameters, ct);
			await Session.Execute(command);
		}

		private async Task AddLinks(IEnumerable<SubscribedParserLink> links, CancellationToken ct)
		{
			if (!links.Any())
				return;
			const string sql = """
				INSERT INTO parsers_control_module.parser_links (id, parser_id, name, url, processed, elapsed_seconds, is_active) 
				VALUES (@id, @parser_id, @name, @url, @processed, @elapsed_seconds, @is_active)
				""";
			var parameters = links.Select(l => new
			{
				id = l.Id.Value,
				parser_id = l.ParserId.Value,
				name = l.UrlInfo.Name,
				url = l.UrlInfo.Url,
				processed = l.Statistics.ParsedCount.Value,
				elapsed_seconds = l.Statistics.WorkTime.TotalElapsedSeconds,
				is_active = l.Active,
			});

			NpgsqlConnection connection = await Session.GetConnection(ct);
			await connection.ExecuteAsync(sql, parameters, transaction: Session.Transaction);
		}

		private string FormUpdateClauses(IEnumerable<SubscribedParser> parsers, DynamicParameters parameters)
		{
			IEnumerable<SubscribedParser> tracking = GetTrackingParsers(parsers);
			Guid[] ids = [.. tracking.Select(p => p.Id.Value)];
			List<string> setClauses = [];

			if (tracking.Any(u => u.State.Value != _trackingParsers[u.Id.Value].State.Value))
			{
				string caseWhen = string.Join(
					" ",
					tracking.Select(
						(p, i) =>
						{
							string paramName = $"@state_{i}";
							parameters.Add(paramName, p.State.Value, DbType.String);
							return $"{CreateParserWhenClause(i)} THEN {paramName}";
						}
					)
				);
				setClauses.Add($"state = CASE {caseWhen} ELSE state END");
			}

			if (tracking.Any(u => u.Schedule.FinishedAt != _trackingParsers[u.Id.Value].Schedule.FinishedAt))
			{
				string caseWhen = string.Join(
					" ",
					tracking.Select(
						(p, i) =>
						{
							string paramName = $"@finished_at_{i}";
							parameters.Add(
								paramName,
								p.Schedule.FinishedAt.HasValue ? p.Schedule.FinishedAt.Value : DBNull.Value,
								DbType.DateTime
							);
							return $"{CreateParserWhenClause(i)} THEN {paramName}";
						}
					)
				);
				setClauses.Add($"finished_at = CASE {caseWhen} ELSE finished_at END");
			}

			if (tracking.Any(u => u.Schedule.NextRun != _trackingParsers[u.Id.Value].Schedule.NextRun))
			{
				string caseWhen = string.Join(
					" ",
					tracking.Select(
						(p, i) =>
						{
							string paramName = $"@next_run_{i}";
							parameters.Add(
								paramName,
								p.Schedule.NextRun.HasValue ? p.Schedule.NextRun.Value : DBNull.Value,
								DbType.DateTime
							);
							return $"{CreateParserWhenClause(i)} THEN {paramName}";
						}
					)
				);
				setClauses.Add($"next_run = CASE {caseWhen} ELSE next_run END");
			}

			if (tracking.Any(u => u.Schedule.WaitDays != _trackingParsers[u.Id.Value].Schedule.WaitDays))
			{
				string caseWhen = string.Join(
					" ",
					tracking.Select(
						(p, i) =>
						{
							string paramName = $"@wait_days_{i}";
							parameters.Add(
								paramName,
								p.Schedule.WaitDays.HasValue ? p.Schedule.WaitDays.Value : DBNull.Value,
								DbType.Int32
							);
							return $"{CreateParserWhenClause(i)} THEN {paramName}";
						}
					)
				);
				setClauses.Add($"wait_days = CASE {caseWhen} ELSE wait_days END");
			}

			if (tracking.Any(u => u.Schedule.StartedAt != _trackingParsers[u.Id.Value].Schedule.StartedAt))
			{
				string caseWhen = string.Join(
					" ",
					tracking.Select(
						(p, i) =>
						{
							string paramName = $"@started_at_{i}";
							parameters.Add(
								paramName,
								p.Schedule.StartedAt.HasValue ? p.Schedule.StartedAt.Value : DBNull.Value,
								DbType.DateTime
							);
							return $"{CreateParserWhenClause(i)} THEN {paramName}";
						}
					)
				);
				setClauses.Add($"started_at = CASE {caseWhen} ELSE started_at END");
			}

			if (
				tracking.Any(u =>
					u.Statistics.ParsedCount.Value != _trackingParsers[u.Id.Value].Statistics.ParsedCount.Value
				)
			)
			{
				string caseWhen = string.Join(
					" ",
					tracking.Select(
						(p, i) =>
						{
							string paramName = $"@processed_{i}";
							parameters.Add(paramName, p.Statistics.ParsedCount.Value, DbType.Int32);
							return $"{CreateParserWhenClause(i)} THEN {paramName}";
						}
					)
				);
				setClauses.Add($"processed = CASE {caseWhen} ELSE processed END");
			}

			if (
				tracking.Any(u =>
					u.Statistics.WorkTime.TotalElapsedSeconds
					!= _trackingParsers[u.Id.Value].Statistics.WorkTime.TotalElapsedSeconds
				)
			)
			{
				string caseWhen = string.Join(
					" ",
					tracking.Select(
						(p, i) =>
						{
							string paramName = $"@elapsed_seconds_{i}";
							parameters.Add(paramName, p.Statistics.WorkTime.TotalElapsedSeconds, DbType.Int64);
							return $"{CreateParserWhenClause(i)} THEN {paramName}";
						}
					)
				);
				setClauses.Add($"elapsed_seconds = CASE {caseWhen} ELSE elapsed_seconds END");
			}

			if (setClauses.Count == 0)
				return string.Empty;

			for (int i = 0; i < ids.Length; i++)
			{
				Guid value = ids[i];
				string paramName = $"@id_{i}";
				parameters.Add(paramName, value, DbType.Guid);
			}

			parameters.Add("@ids", ids);
			return $"""
				UPDATE parsers_control_module.registered_parsers p
				SET {string.Join(", ", setClauses)}
				WHERE p.id = ANY (@ids)
				""";
		}

		private List<SubscribedParser> GetTrackingParsers(IEnumerable<SubscribedParser> parsers)
		{
			List<SubscribedParser> updated = [];
			foreach (SubscribedParser parser in parsers)
			{
				if (!_trackingParsers.TryGetValue(parser.Id.Value, out _))
					continue;
				updated.Add(parser);
			}
			return updated;
		}

		private static string CreateLinkWhenClause(int index) => $"WHEN pl.id = @id_{index}";

		private static string CreateParserWhenClause(int index) => $"WHEN p.id = @id_{index}";
	}
}
