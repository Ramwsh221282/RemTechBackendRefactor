using RemTech.Core.Shared.Exceptions;
using RemTech.ParsedAdvertisements.Core.Tests.Fixtures;
using RemTech.ParsedAdvertisements.Core.Types.Brands.Decorators.Logic;
using RemTech.ParsedAdvertisements.Core.Types.Brands.Decorators.Postgres;
using RemTech.ParsedAdvertisements.Core.Types.Brands.Decorators.Validation;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Tests.Brands.PgTests;

public sealed class BrandsPgTests(PgTestsFixture fixture) : IClassFixture<PgTestsFixture>
{
    [Theory]
    [InlineData("Ponsse")]
    [InlineData("Ponsse New Brand")]
    private async Task Add_New_Pg_Brand_Success(string name)
    {
        await using PgConnectionSource connectionSource = new(fixture.DbConfig());
        await new PgVarietVehicleBrand(
            connectionSource,
            new ValidVehicleBrand(new NewVehicleBrand(name))
        ).SaveAsync(CancellationToken.None);
    }

    [Fact]
    private async Task Add_New_Pg_Brand_Failure()
    {
        await using PgConnectionSource connectionSource = new(fixture.DbConfig());
        await Assert.ThrowsAsync<ValueNotValidException>(() =>
            new PgVarietVehicleBrand(
                connectionSource,
                new ValidVehicleBrand(new NewVehicleBrand(string.Empty))
            ).SaveAsync(CancellationToken.None)
        );
    }
}
