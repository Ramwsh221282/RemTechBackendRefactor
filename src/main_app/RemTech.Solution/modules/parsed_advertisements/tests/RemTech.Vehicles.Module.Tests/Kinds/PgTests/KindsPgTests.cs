using RemTech.Core.Shared.Exceptions;
using RemTech.Postgres.Adapter.Library;
using RemTech.Vehicles.Module.Tests.Fixtures;
using RemTech.Vehicles.Module.Types.Kinds;
using RemTech.Vehicles.Module.Types.Kinds.Decorators.Logic;
using RemTech.Vehicles.Module.Types.Kinds.Decorators.Postgres;
using RemTech.Vehicles.Module.Types.Kinds.Decorators.Validation;

namespace RemTech.Vehicles.Module.Tests.Kinds.PgTests;

public sealed class KindsPgTests(PgTestsFixture fixture) : IClassFixture<PgTestsFixture>
{
    [Theory]
    [InlineData("Форвардер")]
    [InlineData("Новый Тип Техники")]
    private async Task Add_Kind_Success(string kindName)
    {
        await using PgConnectionSource source = new(fixture.DbConfig());
        await new PgVarietVehicleKind(
            source,
            new ValidVehicleKind(new VehicleKind(new NewVehicleKind(kindName)))
        ).SaveAsync(CancellationToken.None);
        ;
    }

    [Fact]
    private async Task Add_Kind_Failure()
    {
        await using PgConnectionSource source = new(fixture.DbConfig());
        await Assert.ThrowsAnyAsync<ValueNotValidException>(() =>
            new PgVarietVehicleKind(
                source,
                new ValidVehicleKind(new VehicleKind(new NewVehicleKind(string.Empty)))
            ).SaveAsync(CancellationToken.None)
        );
    }
}
