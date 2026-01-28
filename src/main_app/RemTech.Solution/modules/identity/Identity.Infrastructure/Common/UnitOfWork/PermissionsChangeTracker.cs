using System.Data;
using Dapper;
using Identity.Domain.Permissions;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Identity.Infrastructure.Common.UnitOfWork;

/// <summary>
/// Трекер изменений разрешений.
/// </summary>
/// <param name="session">Сессия базы данных для выполнения операций.</param>
public sealed class PermissionsChangeTracker(NpgSqlSession session)
{
	private readonly Dictionary<Guid, Permission> _tracking = [];
	private NpgSqlSession Session { get; } = session;

	/// <summary>
	/// Сохраняет изменения для коллекции разрешений.
	/// </summary>
	/// <param name="permissions">Коллекция разрешений для сохранения изменений.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию сохранения изменений.</returns>
	public async Task SaveChanges(IEnumerable<Permission> permissions, CancellationToken ct)
	{
		IEnumerable<Permission> tracking = GetTrackingPermissions(permissions);
		if (!tracking.Any())
			return;
		await SavePermissionChanges(tracking, ct);
	}

	/// <summary>
	/// Начинает отслеживание изменений для коллекции разрешений.
	/// </summary>
	/// <param name="permissions">Коллекция разрешений для отслеживания изменений.</param>
	public void StartTracking(IEnumerable<Permission> permissions)
	{
		foreach (Permission permission in permissions)
			_tracking.TryAdd(permission.Id.Value, permission.Clone());
	}

	private static string WhenClause(int index) => $"WHEN p.id = @permission_id_{index}";

	private async Task SavePermissionChanges(IEnumerable<Permission> permissions, CancellationToken ct)
	{
		List<string> setClauses = [];
		DynamicParameters parameters = new();
		Permission[] permissionsArray = [.. permissions];

		if (permissionsArray.Any(p => p.Name.Value != _tracking[p.Id.Value].Name.Value))
		{
			string clause = string.Join(
				" ",
				permissionsArray.Select(
					(p, i) =>
					{
						string paramName = $"@name_{i}";
						parameters.Add(paramName, p.Name.Value, DbType.String);
						return $"{WhenClause(i)} THEN {paramName}";
					}
				)
			);
			setClauses.Add($"name = CASE {clause} ELSE name END");
		}

		if (permissionsArray.Any(p => p.Description.Value != _tracking[p.Id.Value].Description.Value))
		{
			string clause = string.Join(
				" ",
				permissionsArray.Select(
					(p, i) =>
					{
						string paramName = $"@description_{i}";
						parameters.Add(paramName, p.Description.Value, DbType.String);
						return $"{WhenClause(i)} THEN {paramName}";
					}
				)
			);
			setClauses.Add($"description = CASE {clause} ELSE description END");
		}

		if (setClauses.Count == 0)
			return;

		List<Guid> ids = [];
		int index = 0;
		foreach (Permission permission in permissionsArray)
		{
			string paramName = $"@permission_id_{index}";
			parameters.Add(paramName, permission.Id.Value, DbType.Guid);
			ids.Add(permission.Id.Value);
			index++;
		}

		parameters.Add("@ids", ids.ToArray());

		string updateSql = $"""
			UPDATE identity_module.permissions p
			SET {string.Join(", ", setClauses)}
			WHERE p.id = ANY (@ids)
			""";

		CommandDefinition command = Session.FormCommand(updateSql, parameters, ct);
		await Session.Execute(command);
	}

	private List<Permission> GetTrackingPermissions(IEnumerable<Permission> permissions)
	{
		List<Permission> tracking = [];
		foreach (Permission permission in permissions)
		{
			if (_tracking.ContainsKey(permission.Id.Value))
				tracking.Add(permission);
		}

		return tracking;
	}
}
