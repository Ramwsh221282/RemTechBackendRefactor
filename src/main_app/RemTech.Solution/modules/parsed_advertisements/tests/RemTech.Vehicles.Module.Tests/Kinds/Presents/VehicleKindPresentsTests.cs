using Npgsql;
using RemTech.Postgres.Adapter.Library;
using RemTech.Vehicles.Module.Features.QueryVehicleKinds;
using RemTech.Vehicles.Module.Features.QueryVehicleKinds.Types;
using RemTech.Vehicles.Module.Tests.Fixtures;

namespace RemTech.Vehicles.Module.Tests.Kinds.Presents;

public sealed class VehicleKindPresentsTests(PgTestsFixture fixture) : IClassFixture<PgTestsFixture>
{
    [Fact]
    private async Task ReadPresents()
    {
        await using PgConnectionSource source = new PgConnectionSource(fixture.DbConfig());
        await using NpgsqlConnection connection = await source.Connect();
        IEnumerable<VehicleKindPresentation> kinds = await connection.Provide(
            VehicleKindsPresentationSource.VehicleKindsCommand,
            VehicleKindsPresentationSource.VehicleKindsReader,
            VehicleKindsPresentationSource.VehicleKindsReading,
            CancellationToken.None
        );
        Assert.NotEmpty(kinds);
    }
}
