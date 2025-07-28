using RemTech.Logging.Adapter;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Arguments;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Presenting;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehicleBrandPresentation;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehicleKindsPresentation;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehicleModelsPresentation;
using RemTech.ParsedAdvertisements.Core.Tests.Fixtures;
using RemTech.Postgres.Adapter.Library;
using Serilog;

namespace RemTech.ParsedAdvertisements.Core.Tests.Vehicles.Features;

public sealed class QueryVehiclesTests(PgTestsFixture fixture) : IClassFixture<PgTestsFixture>
{
    [Fact]
    private async Task Query_Vehicles_Standard_Success()
    {
        await using PgConnectionSource connectionSource = new PgConnectionSource(fixture.DbConfig());
        ILogger logger = new LoggerSource().Logger();
        IEnumerable<VehicleKindPresent> kinds = await new VehicleKindPresentsSource(connectionSource)
            .ReadAsync();
        VehicleKindPresent firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresent> brands = await new VehicleBrandPresentsSource(kindId, connectionSource)
            .ReadAsync();
        VehicleBrandPresent firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresent> models = await new VehicleModelPresentsSource(brandId, kindId, connectionSource)
            .ReadAsync(CancellationToken.None);
        VehicleModelPresent firstModel = models.First();
        Guid modelId = firstModel.Id;
        int page = 1;
        PgVehiclesProvider provider = new PgVehiclesProvider(
            connectionSource, 
            logger,
            new VehiclesQueryRequest(
                new VehicleKindIdQueryFilterArgument(kindId),
                new VehicleBrandIdQueryFilterArgument(brandId),
                new VehicleModelIdQueryFilterArgument(modelId),
                new VehiclePaginationQueryFilterArgument(page)));
        IEnumerable<VehiclePresentation> vehicles = await provider.Provide(CancellationToken.None);
        Assert.NotEmpty(vehicles);
    }

    [Fact]
    private async Task Query_Vehicles_With_Location_Success()
    {
        await using PgConnectionSource connectionSource = new PgConnectionSource(fixture.DbConfig());
        ILogger logger = new LoggerSource().Logger();
        IEnumerable<VehicleKindPresent> kinds = await new VehicleKindPresentsSource(connectionSource)
            .ReadAsync();
        VehicleKindPresent firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresent> brands = await new VehicleBrandPresentsSource(kindId, connectionSource)
            .ReadAsync();
        VehicleBrandPresent firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresent> models = await new VehicleModelPresentsSource(brandId, kindId, connectionSource)
            .ReadAsync(CancellationToken.None);
        VehicleModelPresent firstModel = models.First();
        Guid modelId = firstModel.Id;
        int page = 1;
        PgVehiclesProvider provider = 
            new PgVehiclesProvider(
                connectionSource,
                logger,
                new VehiclesQueryRequest(
                    new VehicleKindIdQueryFilterArgument(kindId),
                    new VehicleBrandIdQueryFilterArgument(brandId),
                    new VehicleModelIdQueryFilterArgument(modelId),
                    new VehiclePaginationQueryFilterArgument(page),
                    RegionId: new VehicleRegionIdQueryFilterArgument(Guid.Parse("74487d05-0066-4528-8524-b3eca2b28624"))));
        IEnumerable<VehiclePresentation> vehicles = await provider.Provide(CancellationToken.None);
        Assert.NotEmpty(vehicles);
    }

    [Fact]
    private async Task Query_Vehicles_With_Price_To_Success()
    {
        await using PgConnectionSource connectionSource = new PgConnectionSource(fixture.DbConfig());
        ILogger logger = new LoggerSource().Logger();
        IEnumerable<VehicleKindPresent> kinds = await new VehicleKindPresentsSource(connectionSource)
            .ReadAsync();
        VehicleKindPresent firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresent> brands = await new VehicleBrandPresentsSource(kindId, connectionSource)
            .ReadAsync();
        VehicleBrandPresent firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresent> models = await new VehicleModelPresentsSource(brandId, kindId, connectionSource)
            .ReadAsync(CancellationToken.None);
        VehicleModelPresent firstModel = models.First();
        Guid modelId = firstModel.Id;
        int page = 1;
        PgVehiclesProvider provider = 
            new PgVehiclesProvider(
                connectionSource,
                logger,
                new VehiclesQueryRequest(
                    new VehicleKindIdQueryFilterArgument(kindId),
                    new VehicleBrandIdQueryFilterArgument(brandId),
                    new VehicleModelIdQueryFilterArgument(modelId),
                    new VehiclePaginationQueryFilterArgument(page),
                    Price: new VehiclePriceQueryFilterArgument(true, 1684900)));
        IEnumerable<VehiclePresentation> vehicles = await provider.Provide(CancellationToken.None);
        Assert.NotEmpty(vehicles);
    }
    
    [Fact]
    private async Task Query_Vehicles_With_Price_From_Success()
    {
        await using PgConnectionSource connectionSource = new PgConnectionSource(fixture.DbConfig());
        ILogger logger = new LoggerSource().Logger();
        IEnumerable<VehicleKindPresent> kinds = await new VehicleKindPresentsSource(connectionSource)
            .ReadAsync();
        VehicleKindPresent firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresent> brands = await new VehicleBrandPresentsSource(kindId, connectionSource)
            .ReadAsync();
        VehicleBrandPresent firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresent> models = await new VehicleModelPresentsSource(brandId, kindId, connectionSource)
            .ReadAsync(CancellationToken.None);
        VehicleModelPresent firstModel = models.First();
        Guid modelId = firstModel.Id;
        int page = 1;
        PgVehiclesProvider provider = 
            new PgVehiclesProvider(
                connectionSource,
                logger,
                new VehiclesQueryRequest(
                    new VehicleKindIdQueryFilterArgument(kindId),
                    new VehicleBrandIdQueryFilterArgument(brandId),
                    new VehicleModelIdQueryFilterArgument(modelId),
                    new VehiclePaginationQueryFilterArgument(page),
                    Price: new VehiclePriceQueryFilterArgument(PriceFrom: 1648900)));
        IEnumerable<VehiclePresentation> vehicles = await provider.Provide(CancellationToken.None);
        Assert.NotEmpty(vehicles);
    }
    
    [Fact]
    private async Task Query_Vehicles_With_Price_Range_Success()
    {
        await using PgConnectionSource connectionSource = new PgConnectionSource(fixture.DbConfig());
        ILogger logger = new LoggerSource().Logger();
        IEnumerable<VehicleKindPresent> kinds = await new VehicleKindPresentsSource(connectionSource)
            .ReadAsync();
        VehicleKindPresent firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresent> brands = await new VehicleBrandPresentsSource(kindId, connectionSource)
            .ReadAsync();
        VehicleBrandPresent firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresent> models = await new VehicleModelPresentsSource(brandId, kindId, connectionSource)
            .ReadAsync(CancellationToken.None);
        VehicleModelPresent firstModel = models.First();
        Guid modelId = firstModel.Id;
        int page = 1;
        PgVehiclesProvider provider = 
            new PgVehiclesProvider(
                connectionSource,
                logger,
                new VehiclesQueryRequest(
                    new VehicleKindIdQueryFilterArgument(kindId),
                    new VehicleBrandIdQueryFilterArgument(brandId),
                    new VehicleModelIdQueryFilterArgument(modelId),
                    new VehiclePaginationQueryFilterArgument(page),
                    Price: new VehiclePriceQueryFilterArgument(PriceFrom: 1648900, PriceTo: 1684900)));
        IEnumerable<VehiclePresentation> vehicles = await provider.Provide(CancellationToken.None);
        Assert.NotEmpty(vehicles);
    }

    [Fact]
    private async Task Query_Vehicle_With_Characteristics_Success()
    {
        await using PgConnectionSource connectionSource = new PgConnectionSource(fixture.DbConfig());
        ILogger logger = new LoggerSource().Logger();
        IEnumerable<VehicleKindPresent> kinds = await new VehicleKindPresentsSource(connectionSource)
            .ReadAsync();
        VehicleKindPresent firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresent> brands = await new VehicleBrandPresentsSource(kindId, connectionSource)
            .ReadAsync();
        VehicleBrandPresent firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresent> models = await new VehicleModelPresentsSource(brandId, kindId, connectionSource)
            .ReadAsync(CancellationToken.None);
        VehicleModelPresent firstModel = models.First();
        Guid modelId = firstModel.Id;
        int page = 1;
        PgVehiclesProvider provider = 
            new PgVehiclesProvider(
                connectionSource,
                logger,
                new VehiclesQueryRequest(
                    new VehicleKindIdQueryFilterArgument(kindId),
                    new VehicleBrandIdQueryFilterArgument(brandId),
                    new VehicleModelIdQueryFilterArgument(modelId),
                    new VehiclePaginationQueryFilterArgument(page),
                    Characteristics: new VehicleCharacteristicsQueryArguments([new VehicleCharacteristicQueryArgument(
                        Guid.Parse("9005d171-baf2-45af-a00e-e07a427968c4"), "Высота выгрузки", "3000")])));
        IEnumerable<VehiclePresentation> vehicles = await provider.Provide(CancellationToken.None);
        Assert.NotEmpty(vehicles);
    }
}