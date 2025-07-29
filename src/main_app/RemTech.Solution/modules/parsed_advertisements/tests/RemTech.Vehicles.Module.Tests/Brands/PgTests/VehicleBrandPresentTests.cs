using RemTech.Postgres.Adapter.Library;
using RemTech.Vehicles.Module.Features.VehicleBrandPresentation;
using RemTech.Vehicles.Module.Features.VehicleKindsPresentation;
using RemTech.Vehicles.Module.Tests.Fixtures;

namespace RemTech.Vehicles.Module.Tests.Brands.PgTests;

public sealed class VehicleBrandPresentTests(PgTestsFixture fixture) : IClassFixture<PgTestsFixture>
{
    [Fact]
    private async Task Read_Brand_Presents_Success()
    {
        await using PgConnectionSource source = new PgConnectionSource(fixture.DbConfig());
        IEnumerable<VehicleKindPresent> kinds = await new VehicleKindPresentsSource(
            source
        ).ReadAsync();
        VehicleKindPresent firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresent> presents = await new VehicleBrandPresentsSource(
            kindId,
            source
        ).ReadAsync();
        Assert.NotEmpty(presents);
    }
}
