using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Ports.Storage;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Adapters.Storage.Postgres;

public sealed class PgVehicleBrandStoreCommand(VehicleBrandIdentity identity) : IPgVehicleBrandStoreCommand
{
    private readonly string _sql = string.Intern("""
                                                 INSERT INTO parsed_advertisements_module.vehicle_brands(id, text)
                                                 VALUES(@id, @text)
                                                 ON CONFLICT(text) DO NOTHING;
                                                 """);
    public async  Task<int> Execute(NpgsqlConnection connection, CancellationToken ct = default)
    {
        if (!identity.ReadText())
            throw new ArgumentException("Vehicle identity name is empty");
        if (!identity.ReadId())
            throw new ArgumentException("Vehicle identity ID is empty");
        return await new AsyncExecutedCommand(new AsyncPreparedCommand(
            new ParametrizingPgCommand(new PgCommand(connection, _sql))
                .With("@id", (Guid)identity.ReadId())
                .With("@text", (string)identity.ReadText()))).AsyncExecuted(ct);
    }
}