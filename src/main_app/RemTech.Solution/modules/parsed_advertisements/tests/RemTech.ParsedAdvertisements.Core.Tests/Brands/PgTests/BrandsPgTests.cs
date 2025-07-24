using System.Diagnostics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Ports.Storage;
using RemTech.ParsedAdvertisements.Core.Tests.Fixtures;
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
        IPgVehicleBrandsStorage storage = new PgVarietVehicleBrandsStorage()
            .With(new PgLoggingVehicleBrandsStorage(fixture.Logger(), 
                new PgVehicleBrandsStore(connectionSource)))
            .With(new PgLoggingVehicleBrandsStorage(fixture.Logger(), 
                new PgDuplicateResolvingVehicleBrandsStore(connectionSource)));
        VehicleBrand brand = new NewVehicleBrand(name);
        await storage.Get(brand, CancellationToken.None);
    }
    
    [Fact]
    private async Task Add_New_Pg_Brand_Failure()
    {
        await using PgConnectionSource connectionSource = new(fixture.DbConfig());
        IPgVehicleBrandsStorage storage = new PgVarietVehicleBrandsStorage()
            .With(new PgLoggingVehicleBrandsStorage(fixture.Logger(), 
                new PgVehicleBrandsStore(connectionSource)))
            .With(new PgLoggingVehicleBrandsStorage(fixture.Logger(), 
                new PgDuplicateResolvingVehicleBrandsStore(connectionSource)));
        VehicleBrand brand = new NewVehicleBrand(string.Empty);
        await Assert.ThrowsAsync<UnreachableException>(() =>  storage.Get(brand, CancellationToken.None));
    }
}