using System.Diagnostics;
using RemTech.Core.Shared.Primitives;
using RemTech.Postgres.Adapter.Library;
using RemTech.Vehicles.Module.Tests.Fixtures;
using RemTech.Vehicles.Module.Types.Characteristics.Adapters.Storage.Postgres;
using RemTech.Vehicles.Module.Types.Characteristics.Features.Structuring;
using RemTech.Vehicles.Module.Types.Characteristics.Ports.Storage;

namespace RemTech.Vehicles.Module.Tests.Characteristics.PgTests;

public sealed class CharacteristicsPgTests(PgTestsFixture fixture) : IClassFixture<PgTestsFixture>
{
    [Theory]
    [InlineData("Год выпуска", "Год выпуска: 2025")]
    [InlineData("Мощность двигателя", "50 л.с")]
    [InlineData("Эксплуатационная масса", "3300 кг")]
    [InlineData("Грузоподъёмность", "3300 кг")]
    [InlineData("Высота выгрузки", "3000 мм")]
    private async Task Add_Ctx_Success(string name, string value)
    {
        await using PgConnectionSource source = new PgConnectionSource(fixture.DbConfig());
        IPgCharacteristicsStorage storage = new PgVarietCharacteristicsStorage()
            .With(new PgCharacteristicsStorage(source))
            .With(new PgDuplicateResolvingCharacteristicsStorage(source));
        await storage.Stored(
            new CharacteristicVeil(
                new NotEmptyString(name),
                new NotEmptyString(value)
            ).Characteristic(),
            CancellationToken.None
        );
    }

    [Fact]
    private async Task Add_Ctx_Failure()
    {
        await using PgConnectionSource source = new PgConnectionSource(fixture.DbConfig());
        IPgCharacteristicsStorage storage = new PgVarietCharacteristicsStorage()
            .With(new PgCharacteristicsStorage(source))
            .With(new PgDuplicateResolvingCharacteristicsStorage(source));
        await Assert.ThrowsAnyAsync<UnreachableException>(() =>
            storage.Stored(
                new CharacteristicVeil(
                    new NotEmptyString(string.Empty),
                    new NotEmptyString(string.Empty)
                ).Characteristic(),
                CancellationToken.None
            )
        );
    }
}
