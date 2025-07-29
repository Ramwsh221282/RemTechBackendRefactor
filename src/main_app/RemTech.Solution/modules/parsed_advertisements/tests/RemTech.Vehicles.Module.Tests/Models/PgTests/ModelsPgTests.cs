using RemTech.Core.Shared.Exceptions;
using RemTech.Postgres.Adapter.Library;
using RemTech.Vehicles.Module.Tests.Fixtures;
using RemTech.Vehicles.Module.Types.Models;
using RemTech.Vehicles.Module.Types.Models.Decorators.Postgres;
using RemTech.Vehicles.Module.Types.Models.Decorators.Validation;
using RemTech.Vehicles.Module.Types.Models.ValueObjects;

namespace RemTech.Vehicles.Module.Tests.Models.PgTests;

public sealed class ModelsPgTests(PgTestsFixture fixture) : IClassFixture<PgTestsFixture>
{
    [Theory]
    [InlineData("АВД МЗВЫФ")]
    [InlineData("Bufallo")]
    private async Task Add_Model_Success(string name)
    {
        await using PgConnectionSource source = new PgConnectionSource(fixture.DbConfig());
        await new PgVarietVehicleModel(
            source,
            new ValidVehicleModel(
                new VehicleModel(
                    new VehicleModelIdentity(Guid.NewGuid()),
                    new VehicleModelName(name)
                )
            )
        ).SaveAsync(CancellationToken.None);
    }

    [Fact]
    private async Task Add_Model_Failure()
    {
        await using PgConnectionSource source = new PgConnectionSource(fixture.DbConfig());
        await Assert.ThrowsAnyAsync<ValueNotValidException>(() =>
            new PgVarietVehicleModel(
                source,
                new ValidVehicleModel(
                    new VehicleModel(
                        new VehicleModelIdentity(Guid.NewGuid()),
                        new VehicleModelName(string.Empty)
                    )
                )
            ).SaveAsync(CancellationToken.None)
        );
    }
}
