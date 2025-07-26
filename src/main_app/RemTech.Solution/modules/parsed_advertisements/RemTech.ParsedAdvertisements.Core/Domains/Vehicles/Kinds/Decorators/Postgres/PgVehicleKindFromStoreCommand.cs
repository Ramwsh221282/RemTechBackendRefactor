using System.Data.Common;
using Npgsql;
using RemTech.Core.Shared.Exceptions;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators.Postgres;

public sealed class PgVehicleKindFromStoreCommand(string text)
{
    private readonly string _sql = string.Intern("""
                                                 SELECT id, text FROM parsed_advertisements_module.vehicle_kinds
                                                 WHERE text = @text
                                                 """);

    public async Task<VehicleKind> Fetch(NpgsqlConnection connection, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new OperationException("Невозможно получить тип техники поиском. Параметр названия был пустым.");
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(
                        new PgCommand(connection, _sql))
                    .With("@text", text))).AsyncReader(ct);
        return await new PgSingleRiddenVehicleKind(reader).Read();
    }
}