using System.Diagnostics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Ports.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Tests.Fixtures;
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
        IPgVehicleKindsStorage storage = new PgVarietVehicleKindStorage()
            .With(new PgVehicleKindsStorage(source))
            .With(new PgDuplicateResolvingVehicleKindStorage(source));
        VehicleKind kind = new NewVehicleKind(kindName);
        await storage.Read(kind, CancellationToken.None);
    }

    [Fact]
    private async Task Add_Kind_Failure()
    {
        await using PgConnectionSource source = new(fixture.DbConfig());
        IPgVehicleKindsStorage storage = new PgVarietVehicleKindStorage()
            .With(new PgVehicleKindsStorage(source))
            .With(new PgDuplicateResolvingVehicleKindStorage(source));
        VehicleKind kind = new NewVehicleKind(string.Empty);
        await Assert.ThrowsAnyAsync<UnreachableException>(() => storage.Read(kind, CancellationToken.None));
    }
}