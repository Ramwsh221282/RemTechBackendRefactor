using Npgsql;
using RemTech.Postgres.Adapter.Library;
using RemTech.Vehicles.Module.Features.QueryVehicleBrands;
using RemTech.Vehicles.Module.Features.QueryVehicleBrands.Types;
using RemTech.Vehicles.Module.Features.QueryVehicleKinds;
using RemTech.Vehicles.Module.Features.QueryVehicleKinds.Types;
using RemTech.Vehicles.Module.Tests.Fixtures;

namespace RemTech.Vehicles.Module.Tests.Brands.PgTests;

public sealed class VehicleBrandPresentationTests(PgTestsFixture fixture)
    : IClassFixture<PgTestsFixture>
{
    [Fact]
    private async Task Read_Brand_Presents_Success()
    {
        await using PgConnectionSource source = new PgConnectionSource(fixture.DbConfig());
        await using NpgsqlConnection connection = await source.Connect();
        IEnumerable<VehicleKindPresentation> kinds = await connection.Provide(
            VehicleKindsPresentationSource.VehicleKindsCommand,
            VehicleKindsPresentationSource.VehicleKindsReader,
            VehicleKindsPresentationSource.VehicleKindsReading,
            CancellationToken.None
        );
        VehicleKindPresentation firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresentation> brands = await connection.Provide(
            kindId,
            VehicleBrandPresentationSource.CreateCommand,
            VehicleBrandPresentationSource.CreateReader,
            VehicleBrandPresentationSource.ProcessWithReader
        );
        Assert.NotEmpty(brands);
    }
}
