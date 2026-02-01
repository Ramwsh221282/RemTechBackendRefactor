using System.Data;
using Dapper;
using Identity.Domain.Accounts.Models;
using Identity.Domain.Permissions;
using Npgsql;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Identity.Infrastructure.Common.UnitOfWork;

/// <summary>
/// Трекер изменений аккаунтов.
/// </summary>
/// <param name="session">Сессия базы данных.</param>
public sealed class AccountsChangeTracker(NpgSqlSession session)
{
	private NpgSqlSession Session { get; } = session;
	private Dictionary<Guid, Account> Accounts { get; } = [];

	/// <summary>
	/// Начинает отслеживание изменений для коллекции аккаунтов.
	/// </summary>
	/// <param name="accounts">Коллекция аккаунтов для отслеживания.</param>
	public void StartTracking(IEnumerable<Account> accounts)
	{
		foreach (Account account in accounts)
		{
			Accounts.TryAdd(account.Id.Value, account.Copy());
		}
	}

	/// <summary>
	/// Сохраняет изменения для коллекции аккаунтов.
	/// </summary>
	/// <param name="accounts">Коллекция аккаунтов для сохранения изменений.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию сохранения изменений.</returns>
	public async Task SaveChanges(IEnumerable<Account> accounts, CancellationToken ct)
	{
		IEnumerable<Account> tracking = GetTrackingAccounts(accounts);
		if (!tracking.Any())
		{
			return;
		}

		await SavePermissionChanges(tracking, ct);
		await SaveAccountChanges(tracking, ct);
	}

	private static string WhenById(int index)
	{
		return $"WHEN a.id = @id_{index}";
	}

	private static (AccountId AccountId, IEnumerable<Permission> Permissions) GetRemovedPermissions(
		Account account,
		Account original
	)
	{
		return (
			account.Id,
			original.Permissions.Permissions.ExceptBy(
				account.Permissions.Permissions.Select(p => p.Id.Value),
				p => p.Id.Value
			)
		);
	}

	private static (AccountId AccountId, IEnumerable<Permission> Permissions) GetAddedPermissions(
		Account account,
		Account original
	)
	{
		return (
			account.Id,
			account.Permissions.Permissions.ExceptBy(
				original.Permissions.Permissions.Select(p => p.Id.Value),
				p => p.Id.Value
			)
		);
	}

	private async Task SavePermissionChanges(IEnumerable<Account> tracking, CancellationToken ct)
	{
		foreach (Account account in tracking)
		{
			Account original = Accounts[account.Id.Value];
			(AccountId, IEnumerable<Permission>) removed = GetRemovedPermissions(account, original);
			(AccountId, IEnumerable<Permission>) added = GetAddedPermissions(account, original);
			await AddNewPermissions(added, ct);
			await DeleteRemovedPermissions(removed, ct);
		}
	}

	private async Task AddNewPermissions(
		(AccountId AccountId, IEnumerable<Permission> Permissions) added,
		CancellationToken ct
	)
	{
		if (!added.Permissions.Any())
		{
			return;
		}

		var parameters = added.Permissions.Select(p => new
		{
			account_id = added.AccountId.Value,
			permission_id = p.Id.Value,
		});

		const string sql = """
			INSERT INTO identity_module.account_permissions (account_id, permission_id)
			VALUES (@account_id, @permission_id)
			""";

		NpgsqlConnection connection = await Session.GetConnection(ct);
		await connection.ExecuteAsync(sql, parameters, transaction: Session.Transaction);
	}

	private async Task DeleteRemovedPermissions(
		(AccountId AccountId, IEnumerable<Permission> Permissions) removed,
		CancellationToken ct
	)
	{
		if (!removed.Permissions.Any())
		{
			return;
		}

		Guid[] permissionIds = [.. removed.Permissions.Select(p => p.Id.Value)];
		Guid accountId = removed.AccountId.Value;
		const string sql = """
			DELETE FROM identity_module.account_permissions
			WHERE account_id = @accountId AND permission_id = ANY(@permissionIds)
			""";
		DynamicParameters parameters = new();
		parameters.Add("@accountId", accountId, DbType.Guid);
		parameters.Add("@permissionIds", permissionIds);
		CommandDefinition command = new(sql, parameters, transaction: Session.Transaction, cancellationToken: ct);
		await Session.Execute(command);
	}

	private async Task SaveAccountChanges(IEnumerable<Account> tracking, CancellationToken ct)
	{
		Account[] trackingArray = [.. tracking];
		List<string> caseSet = [];
		DynamicParameters parameters = new();

		if (trackingArray.Any(a => a.Login.Value != Accounts[a.Id.Value].Login.Value))
		{
			string clause = string.Join(
				" ",
				trackingArray.Select(
					(a, i) =>
					{
						string when = $"{WhenById(i)} THEN @login_{i}";
						parameters.Add($"@login_{i}", a.Login.Value, DbType.String);
						return when;
					}
				)
			);
			caseSet.Add($"login = CASE {clause} ELSE login END");
		}

		if (trackingArray.Any(a => a.Email.Value != Accounts[a.Id.Value].Email.Value))
		{
			string clause = string.Join(
				" ",
				trackingArray.Select(
					(a, i) =>
					{
						string when = $"{WhenById(i)} THEN @email_{i}";
						parameters.Add($"@email_{i}", a.Email.Value, DbType.String);
						return when;
					}
				)
			);
			caseSet.Add($"email = CASE {clause} ELSE email END");
		}

		if (trackingArray.Any(a => a.ActivationStatus.Value != Accounts[a.Id.Value].ActivationStatus.Value))
		{
			string clause = string.Join(
				" ",
				trackingArray.Select(
					(a, i) =>
					{
						string when = $"{WhenById(i)} THEN @activation_status_{i}";
						parameters.Add($"@activation_status_{i}", a.ActivationStatus.Value, DbType.Boolean);
						return when;
					}
				)
			);
			caseSet.Add($"activation_status = CASE {clause} ELSE activation_status END");
		}

		if (trackingArray.Any(a => a.Password.Value != Accounts[a.Id.Value].Password.Value))
		{
			string clause = string.Join(
				" ",
				trackingArray.Select(
					(a, i) =>
					{
						string when = $"{WhenById(i)} THEN @password_{i}";
						parameters.Add($"@password_{i}", a.Password.Value, DbType.String);
						return when;
					}
				)
			);
			caseSet.Add($"password = CASE {clause} ELSE password END");
		}

		if (caseSet.Count == 0)
		{
			return;
		}

		List<Guid> ids = [];
		int index = 0;
		foreach (Account account in trackingArray)
		{
			string paramName = $"@id_{index}";
			parameters.Add(paramName, account.Id.Value, DbType.Guid);
			ids.Add(account.Id.Value);
			index++;
		}

		parameters.Add("@ids", ids.ToArray());
		string updateSql = $"UPDATE identity_module.accounts a SET {string.Join(", ", caseSet)} WHERE a.id = ANY(@ids)";
		CommandDefinition command = Session.FormCommand(updateSql, parameters, ct);
		await Session.Execute(command);
	}

	private List<Account> GetTrackingAccounts(IEnumerable<Account> accounts)
	{
		List<Account> tracking = [];
		foreach (Account account in accounts)
		{
			if (Accounts.ContainsKey(account.Id.Value))
			{
				tracking.Add(account);
			}
		}

		return tracking;
	}
}
