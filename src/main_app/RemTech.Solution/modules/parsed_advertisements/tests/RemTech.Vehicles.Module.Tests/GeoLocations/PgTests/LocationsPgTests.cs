using RemTech.Core.Shared.Exceptions;
using RemTech.Postgres.Adapter.Library;
using RemTech.Vehicles.Module.Tests.Fixtures;
using RemTech.Vehicles.Module.Types.GeoLocations;
using RemTech.Vehicles.Module.Types.GeoLocations.Decorators.Logic;
using RemTech.Vehicles.Module.Types.GeoLocations.Decorators.Postgres;
using RemTech.Vehicles.Module.Types.GeoLocations.Decorators.Validation;

namespace RemTech.Vehicles.Module.Tests.GeoLocations.PgTests;

public sealed class LocationsPgTests(PgTestsFixture fixture) : IClassFixture<PgTestsFixture>
{
    [Theory]
    [InlineData("Красноярский", "край")]
    private async Task Add_Location_Success(string name, string kind)
    {
        await using PgConnectionSource source = new(fixture.DbConfig());
        await new PgGeoLocation(
            source,
            new ValidGeoLocation(new GeoLocation(new NewGeoLocation(name, kind)))
        ).SaveAsync(CancellationToken.None);
    }

    [Fact]
    private async Task Add_Location_Failure()
    {
        await using PgConnectionSource source = new(fixture.DbConfig());
        await Assert.ThrowsAnyAsync<ValueNotValidException>(() =>
            new PgGeoLocation(
                source,
                new ValidGeoLocation(
                    new GeoLocation(new NewGeoLocation(string.Empty, string.Empty))
                )
            ).SaveAsync(CancellationToken.None)
        );
    }
}
