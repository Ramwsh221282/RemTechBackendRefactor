using RemTech.Postgres.Adapter.Library;
using RemTech.Vehicles.Module.Features.VehicleBrandPresentation;
using RemTech.Vehicles.Module.Features.VehicleKindsPresentation;
using RemTech.Vehicles.Module.Features.VehicleModelsPresentation;
using RemTech.Vehicles.Module.Tests.Fixtures;

namespace RemTech.Vehicles.Module.Tests.Models.PgTests;

public sealed class ModelsPresentsTesting(PgTestsFixture fixture) : IClassFixture<PgTestsFixture>
{
    [Fact]
    private async Task Read_Model_Presents_Success()
    {
        await using PgConnectionSource source = new PgConnectionSource(fixture.DbConfig());
        IEnumerable<VehicleKindPresent> kinds = await new VehicleKindPresentsSource(
            source
        ).ReadAsync(CancellationToken.None);
        VehicleKindPresent firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresent> brands = await new VehicleBrandPresentsSource(
            kindId,
            source
        ).ReadAsync(CancellationToken.None);
        VehicleBrandPresent firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresent> models = await new VehicleModelPresentsSource(
            brandId,
            kindId,
            source
        ).ReadAsync(CancellationToken.None);
        Assert.NotEmpty(models);
    }
}
