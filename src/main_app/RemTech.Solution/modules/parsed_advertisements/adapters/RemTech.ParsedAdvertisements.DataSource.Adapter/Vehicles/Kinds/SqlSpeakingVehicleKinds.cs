using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;
using RemTech.Postgres.Adapter.Library;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Kinds;

// public sealed class SqlSpeakingVehicleKinds(PgDefaultConnectionSource engine) : ISqlSpeakingKinds
// {
//     public async Task<Status<IVehicleKind>> Add(IVehicleKind kind, CancellationToken ct = default)
//     {
//         Guid id = kind.Identify().ReadId();
//         string text = kind.Identify().ReadText();
//         var parameters = new { id, text };
//         string sql = string.Intern(
//             """
//             INSERT INTO parsed_advertisements_module.vehicle_kinds
//             VALUES (@id, @text)
//             ON CONFLICT (text) DO NOTHING
//             """
//         );
//         CommandDefinition command = new(sql, parameters);
//         await using NpgsqlConnection connection = await engine.Connect(ct);
//         int affected = await connection.ExecuteAsync(command);
//         return affected == 0
//             ? Error.Conflict(
//                 $"Не удается добавить тип техники. Тип техники: {text} уже присутствует."
//             )
//             : kind.Success();
//     }
//
//     public async Task<MaybeBag<IVehicleKind>> Find(
//         VehicleKindIdentity identity,
//         CancellationToken ct = default
//     )
//     {
//         List<string> searchTerms = [];
//         DynamicParameters parameters = new();
//         if (identity.ReadText())
//         {
//             searchTerms.Add("text = @text");
//             parameters.Add("text", (string)identity.ReadText());
//         }
//         if (identity.ReadId())
//         {
//             searchTerms.Add("id = @id");
//             parameters.Add("id", (Guid)identity.ReadId());
//         }
//         if (searchTerms.Count == 0)
//             return new MaybeBag<IVehicleKind>();
//         string sql = string.Intern(
//             $"""
//             SELECT id, text FROM parsed_advertisements_module.vehicle_kinds
//             WHERE {string.Join(" AND ", searchTerms)}
//             """
//         );
//         CommandDefinition command = new(sql, parameters, cancellationToken: ct);
//         await using NpgsqlConnection connection = await engine.Connect(ct);
//         await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
//         return !await reader.ReadAsync(ct)
//             ? new MaybeBag<IVehicleKind>()
//             : new VehicleKindSqlRow(reader).Read().Success();
//     }
//
//     public void Dispose() => engine.Dispose();
//
//     public ValueTask DisposeAsync() => engine.DisposeAsync();
// }
