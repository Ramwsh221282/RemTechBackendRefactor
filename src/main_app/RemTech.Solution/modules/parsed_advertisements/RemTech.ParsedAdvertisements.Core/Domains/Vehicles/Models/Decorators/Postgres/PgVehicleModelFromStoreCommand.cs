using System.Data.Common;
using Npgsql;
using RemTech.Core.Shared.Exceptions;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Decorators.Postgres;

public sealed class PgVehicleModelFromStoreCommand(string name)
{
    private readonly  string _sql = string.Intern("""
                                                 SELECT id, text FROM parsed_advertisements_module.vehicle_models
                                                 WHERE text = @text
                                                 """);

    public async Task<VehicleModel> Fetch(NpgsqlConnection connection, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_sql))
            throw new OperationException("Невозможно найти модель техники поиском. Название модели техники было пустым.");
        await using DbDataReader reader = await new AsyncDbReaderCommand(
                new AsyncPreparedCommand(new ParametrizingPgCommand(new PgCommand(connection, _sql))
                    .With("@text", name)))
            .AsyncReader(ct);
        return await new PgSingleRiddenVehicleModelFromStore(reader).Read();
    }
}