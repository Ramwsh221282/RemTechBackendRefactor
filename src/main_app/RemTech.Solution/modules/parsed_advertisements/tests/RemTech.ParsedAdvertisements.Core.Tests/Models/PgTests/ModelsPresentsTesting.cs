using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehicleBrandPresentation;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehicleKindsPresentation;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehicleModelsPresentation;
using RemTech.ParsedAdvertisements.Core.Tests.Fixtures;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Tests.Models.PgTests;

public sealed class ModelsPresentsTesting(PgTestsFixture fixture) : IClassFixture<PgTestsFixture>
{
    [Fact]
    private async Task Read_Model_Presents_Success()
    {
        await using PgConnectionSource source = new PgConnectionSource(fixture.DbConfig());
        IEnumerable<VehicleKindPresent> kinds = await new VehicleKindPresentsSource(source)
            .ReadAsync(CancellationToken.None);
        VehicleKindPresent firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresent> brands = await new VehicleBrandPresentsSource(kindId, source)
            .ReadAsync(CancellationToken.None);
        VehicleBrandPresent firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresent> models = await new VehicleModelPresentsSource(brandId, kindId, source)
            .ReadAsync(CancellationToken.None);
        Assert.NotEmpty(models);
    }
}