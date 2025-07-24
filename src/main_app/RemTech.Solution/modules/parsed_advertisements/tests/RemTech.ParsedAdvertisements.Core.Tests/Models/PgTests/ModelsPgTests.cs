using System.Diagnostics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Ports.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Tests.Fixtures;
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
        IPgVehicleModelsStorage storage = new PgVarietVehicleModelsStorage()
            .With(new PgLoggingVehicleModelsStorage(fixture.Logger(), new PgVehicleModelsStorage(source)))
            .With(new PgLoggingVehicleModelsStorage(fixture.Logger(), new PgDuplicateResolvingVehicleModelsStorage(source)));
        VehicleModel model = new(new VehicleModelIdentity(Guid.NewGuid()), new VehicleModelName(name));
        await storage.Get(model, CancellationToken.None);
    }

    [Fact]
    private async Task Add_Model_Failure()
    {
        await using PgConnectionSource source = new PgConnectionSource(fixture.DbConfig());
        IPgVehicleModelsStorage storage = new PgVarietVehicleModelsStorage()
            .With(new PgLoggingVehicleModelsStorage(fixture.Logger(), new PgVehicleModelsStorage(source)))
            .With(new PgLoggingVehicleModelsStorage(fixture.Logger(), new PgDuplicateResolvingVehicleModelsStorage(source)));
        VehicleModel model = new(new VehicleModelIdentity(Guid.Empty), new VehicleModelName(string.Empty));
        await Assert.ThrowsAnyAsync<UnreachableException>(() => storage.Get(model, CancellationToken.None));
    }
}