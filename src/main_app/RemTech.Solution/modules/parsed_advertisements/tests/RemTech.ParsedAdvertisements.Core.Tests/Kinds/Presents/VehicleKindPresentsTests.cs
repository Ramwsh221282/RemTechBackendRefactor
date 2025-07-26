using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehicleKindsPresentation;
using RemTech.ParsedAdvertisements.Core.Tests.Fixtures;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Tests.Kinds.Presents;

public sealed class VehicleKindPresentsTests(PgTestsFixture fixture) : IClassFixture<PgTestsFixture>
{
    [Fact]
    private async Task ReadPresents()
    {
        await using PgConnectionSource connectionSource = new PgConnectionSource(fixture.DbConfig());
        IEnumerable<VehicleKindPresent> presents = await new VehicleKindPresentsSource(connectionSource).ReadAsync();
        Assert.NotNull(presents);
    }
}