using RemTech.Postgres.Adapter.Library;
using RemTech.Vehicles.Module.Features.VehicleKindsPresentation;
using RemTech.Vehicles.Module.Tests.Fixtures;

namespace RemTech.Vehicles.Module.Tests.Kinds.Presents;

public sealed class VehicleKindPresentsTests(PgTestsFixture fixture) : IClassFixture<PgTestsFixture>
{
    [Fact]
    private async Task ReadPresents()
    {
        await using PgConnectionSource connectionSource = new PgConnectionSource(
            fixture.DbConfig()
        );
        IEnumerable<VehicleKindPresent> presents = await new VehicleKindPresentsSource(
            connectionSource
        ).ReadAsync();
        Assert.NotNull(presents);
    }
}
