using System.Diagnostics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Ports.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Tests.Fixtures;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Tests.GeoLocations.PgTests;

public sealed class LocationsPgTests(PgTestsFixture fixture) : IClassFixture<PgTestsFixture>
{
    [Theory]
    [InlineData("Красноярский", "край")]
    private async Task Add_Location_Success(string name, string kind)
    {
        await using PgConnectionSource source = new(fixture.DbConfig());
        IPgVehicleGeosStorage storage = new PgVarietVehicleGeosStorage()
            .With(new PgVehicleGeosStorage(source));
        GeoLocation location = new NewGeoLocation(name, kind);
        await storage.Get(location, CancellationToken.None);
    }

    [Fact]
    private async Task Add_Location_Failure()
    {
        await using PgConnectionSource source = new(fixture.DbConfig());
        IPgVehicleGeosStorage storage = new PgVarietVehicleGeosStorage()
            .With(new PgVehicleGeosStorage(source));
        GeoLocation location = new NewGeoLocation(string.Empty, string.Empty);
        await Assert.ThrowsAnyAsync<UnreachableException>(() => storage.Get(location, CancellationToken.None));
    }
}