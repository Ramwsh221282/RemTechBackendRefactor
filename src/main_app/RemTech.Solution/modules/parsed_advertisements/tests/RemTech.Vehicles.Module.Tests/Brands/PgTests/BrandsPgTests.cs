using RemTech.Core.Shared.Exceptions;
using RemTech.Postgres.Adapter.Library;
using RemTech.Vehicles.Module.Tests.Fixtures;
using RemTech.Vehicles.Module.Types.Brands.Decorators.Logic;
using RemTech.Vehicles.Module.Types.Brands.Decorators.Postgres;
using RemTech.Vehicles.Module.Types.Brands.Decorators.Validation;

namespace RemTech.Vehicles.Module.Tests.Brands.PgTests;

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
