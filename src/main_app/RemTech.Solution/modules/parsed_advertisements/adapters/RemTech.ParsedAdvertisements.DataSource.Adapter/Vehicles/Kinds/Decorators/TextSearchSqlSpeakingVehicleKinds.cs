using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Kinds.Decorators;

// public sealed class TextSearchSqlSpeakingVehicleKinds(
//     PgDefaultConnectionSource engine,
//     ISqlSpeakingKinds origin
// ) : ISqlSpeakingKinds
// {
//     public async Task<Status<IVehicleKind>> Add(IVehicleKind kind, CancellationToken ct = default)
//     {
//         string sql = string.Intern(
//             """
//             SELECT id, text FROM parsed_advertisements_module.vehicle_kinds
//             WHERE document_tsvector @@ plainto_tsquery('russian', @name)
//             ORDER BY ts_rank(document_tsvector, plainto_tsquery('russian', @name)) DESC
//             LIMIT 1;
//             """
//         );
//         var parameters = new { name = (string)kind.Identify().ReadText() };
//         CommandDefinition command = new(sql, parameters, cancellationToken: ct);
//         await using NpgsqlConnection connection = await engine.Connect(ct);
//         await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
//         return !await reader.ReadAsync(ct)
//             ? await origin.Add(kind, ct)
//             : new VehicleKindSqlRow(reader).Read().Success();
//     }
//
//     public async Task<MaybeBag<IVehicleKind>> Find(
//         VehicleKindIdentity identity,
//         CancellationToken ct = default
//     )
//     {
//         string name = identity.ReadText();
//         if (string.IsNullOrWhiteSpace(name))
//             return new MaybeBag<IVehicleKind>();
//         string sql = string.Intern(
//             """
//             SELECT id, text FROM parsed_advertisements_module.vehicle_kinds
//             WHERE document_tsvector @@ plainto_tsquery('russian', @name)
//             ORDER BY ts_rank(document_tsvector, plainto_tsquery('russian', @name)) DESC
//             LIMIT 1;
//             """
//         );
//         var parameters = new { name };
//         CommandDefinition command = new(sql, parameters, cancellationToken: ct);
//         await using NpgsqlConnection connection = await engine.Connect(ct);
//         await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
//         return !await reader.ReadAsync(ct)
//             ? await origin.Find(identity, ct)
//             : new VehicleKindSqlRow(reader).Read().Success();
//     }
//
//     public void Dispose() => origin.Dispose();
//
//     public ValueTask DisposeAsync() => origin.DisposeAsync();
// }
