using RemTech.ParsedAdvertisements.Core.Features.VehicleBrandPresentation;
using RemTech.ParsedAdvertisements.Core.Features.VehicleKindsPresentation;
using RemTech.ParsedAdvertisements.Core.Tests.Fixtures;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Tests.Brands.PgTests;

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
