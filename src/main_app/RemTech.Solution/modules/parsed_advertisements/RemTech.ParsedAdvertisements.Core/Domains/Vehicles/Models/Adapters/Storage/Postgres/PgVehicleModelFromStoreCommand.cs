using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Ports.Storage.Postgres;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Adapters.Storage.Postgres;

public sealed class PgVehicleModelFromStoreCommand(string name) : IPgVehicleModelFromStoreCommand
{
    private readonly  string _sql = string.Intern("""
                                                 SELECT id, text FROM parsed_advertisements_module.vehicle_models
                                                 WHERE text = @text
                                                 """);

    public async Task<VehicleModel> Fetch(NpgsqlConnection connection, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_sql))
            throw new ArgumentException("Vehicle model name is empty.");
        await using DbDataReader reader = await new AsyncDbReaderCommand(
                new AsyncPreparedCommand(new ParametrizingPgCommand(new PgCommand(connection, _sql))
                    .With("@text", name)))
            .AsyncReader(ct);
        return await new PgSingleRiddenVehicleModelFromStore(reader).Read();
    }
}