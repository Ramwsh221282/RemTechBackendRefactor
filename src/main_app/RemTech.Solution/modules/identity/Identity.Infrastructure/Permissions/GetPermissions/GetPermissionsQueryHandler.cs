using System.Data.Common;
using Dapper;
using Npgsql;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Identity.Infrastructure.Permissions.GetPermissions;

public sealed class GetPermissionsQueryHandler(NpgSqlSession session)
	: IQueryHandler<GetPermissionsQuery, IReadOnlyList<PermissionResponse>>
{
	public async Task<IReadOnlyList<PermissionResponse>> Handle(
		GetPermissionsQuery query,
		CancellationToken ct = default
	)
	{
		string sql = CreateSql();
		CommandDefinition command = new(sql, cancellationToken: ct);
		NpgsqlConnection connection = await session.GetConnection(ct);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		return await MapFromReader(reader, ct);
	}

	private static async Task<IReadOnlyList<PermissionResponse>> MapFromReader(
		DbDataReader reader,
		CancellationToken ct
	)
	{
		List<PermissionResponse> result = [];

		while (await reader.ReadAsync(ct))
		{
			Guid id = reader.GetGuid(reader.GetOrdinal("id"));
			string name = reader.GetString(reader.GetOrdinal("name"));
			string description = reader.GetString(reader.GetOrdinal("description"));
			result.Add(
				new PermissionResponse()
				{
					Id = id,
					Name = name,
					Description = description,
				}
			);
		}

		return result;
	}

	private static string CreateSql()
	{
		return """
			SELECT
			p.id,
			p.name,
			p.description
			FROM identity_module.permissions p
			""";
	}
}
