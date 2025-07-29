using RemTech.Core.Shared.Exceptions;
using RemTech.ParsedAdvertisements.Core.Tests.Fixtures;
using RemTech.ParsedAdvertisements.Core.Types.Models;
using RemTech.ParsedAdvertisements.Core.Types.Models.Decorators.Postgres;
using RemTech.ParsedAdvertisements.Core.Types.Models.Decorators.Validation;
using RemTech.ParsedAdvertisements.Core.Types.Models.ValueObjects;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Tests.Models.PgTests;

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
