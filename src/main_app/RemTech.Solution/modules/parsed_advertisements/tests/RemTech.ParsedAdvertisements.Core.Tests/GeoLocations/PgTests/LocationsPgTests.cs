using RemTech.Core.Shared.Exceptions;
using RemTech.ParsedAdvertisements.Core.Tests.Fixtures;
using RemTech.ParsedAdvertisements.Core.Types.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Types.GeoLocations.Decorators.Logic;
using RemTech.ParsedAdvertisements.Core.Types.GeoLocations.Decorators.Postgres;
using RemTech.ParsedAdvertisements.Core.Types.GeoLocations.Decorators.Validation;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Tests.GeoLocations.PgTests;

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
