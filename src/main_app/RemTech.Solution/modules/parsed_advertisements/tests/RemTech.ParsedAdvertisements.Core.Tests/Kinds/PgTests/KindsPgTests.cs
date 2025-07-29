using RemTech.Core.Shared.Exceptions;
using RemTech.ParsedAdvertisements.Core.Tests.Fixtures;
using RemTech.ParsedAdvertisements.Core.Types.Kinds;
using RemTech.ParsedAdvertisements.Core.Types.Kinds.Decorators.Logic;
using RemTech.ParsedAdvertisements.Core.Types.Kinds.Decorators.Postgres;
using RemTech.ParsedAdvertisements.Core.Types.Kinds.Decorators.Validation;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Tests.Kinds.PgTests;

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
