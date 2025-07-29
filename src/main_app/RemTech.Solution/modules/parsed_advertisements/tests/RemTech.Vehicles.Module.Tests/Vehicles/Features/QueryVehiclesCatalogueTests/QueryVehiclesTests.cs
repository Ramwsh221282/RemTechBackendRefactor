using Npgsql;
using RemTech.Logging.Adapter;
using RemTech.Postgres.Adapter.Library;
using RemTech.Vehicles.Module.Features.QueryVehicleBrands;
using RemTech.Vehicles.Module.Features.QueryVehicleKinds;
using RemTech.Vehicles.Module.Features.QueryVehicleModels;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Arguments;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Presenting;
using RemTech.Vehicles.Module.Tests.Fixtures;
using Serilog;
using VehicleBrandPresentation = RemTech.Vehicles.Module.Features.QueryVehicleBrands.Types.VehicleBrandPresentation;
using VehicleKindPresentation = RemTech.Vehicles.Module.Features.QueryVehicleKinds.Types.VehicleKindPresentation;
using VehicleModelPresentation = RemTech.Vehicles.Module.Features.QueryVehicleModels.Types.VehicleModelPresentation;

namespace RemTech.Vehicles.Module.Tests.Vehicles.Features.QueryVehiclesCatalogueTests;

public sealed class QueryVehiclesTests(PgTestsFixture fixture) : IClassFixture<PgTestsFixture>
{
    [Fact]
    private async Task Query_Vehicles_Standard_Success()
    {
        ILogger logger = new LoggerSource().Logger();
        await using PgConnectionSource source = new PgConnectionSource(fixture.DbConfig());
        await using NpgsqlConnection connection = await source.Connect();
        IEnumerable<VehicleKindPresentation> kinds = await connection.Provide(
            VehicleKindsPresentationSource.VehicleKindsCommand,
            VehicleKindsPresentationSource.VehicleKindsReader,
            VehicleKindsPresentationSource.VehicleKindsReading,
            CancellationToken.None
        );
        VehicleKindPresentation firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresentation> brands = await connection.Provide(
            kindId,
            VehicleBrandPresentationSource.CreateCommand,
            VehicleBrandPresentationSource.CreateReader,
            VehicleBrandPresentationSource.ProcessWithReader
        );
        VehicleBrandPresentation firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresentation> models = await connection.Provide(
            kindId,
            brandId,
            VehicleModelPresentationSource.VehicleModelsCommandSource,
            VehicleModelPresentationSource.VehicleModelsReaderSource,
            VehicleModelPresentationSource.VehicleModelsReadingSource
        );
        Guid modelId = models.First().Id;
        int page = 1;
        var request = new VehiclesQueryRequest(
            new VehicleKindIdQueryFilterArgument(kindId),
            new VehicleBrandIdQueryFilterArgument(brandId),
            new VehicleModelIdQueryFilterArgument(modelId),
            new VehiclePaginationQueryFilterArgument(page)
        );
        IEnumerable<VehiclePresentation> vehicles = await connection.VehiclesOfCatalogue(
            logger,
            request,
            CancellationToken.None
        )();
        Assert.NotEmpty(vehicles);
    }

    [Fact]
    private async Task Query_Vehicles_With_Location_Success()
    {
        ILogger logger = new LoggerSource().Logger();
        await using PgConnectionSource source = new PgConnectionSource(fixture.DbConfig());
        await using NpgsqlConnection connection = await source.Connect();
        IEnumerable<VehicleKindPresentation> kinds = await connection.Provide(
            VehicleKindsPresentationSource.VehicleKindsCommand,
            VehicleKindsPresentationSource.VehicleKindsReader,
            VehicleKindsPresentationSource.VehicleKindsReading,
            CancellationToken.None
        );
        VehicleKindPresentation firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresentation> brands = await connection.Provide(
            kindId,
            VehicleBrandPresentationSource.CreateCommand,
            VehicleBrandPresentationSource.CreateReader,
            VehicleBrandPresentationSource.ProcessWithReader
        );
        VehicleBrandPresentation firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresentation> models = await connection.Provide(
            kindId,
            brandId,
            VehicleModelPresentationSource.VehicleModelsCommandSource,
            VehicleModelPresentationSource.VehicleModelsReaderSource,
            VehicleModelPresentationSource.VehicleModelsReadingSource
        );
        Guid modelId = models.First().Id;
        int page = 1;
        var request = new VehiclesQueryRequest(
            new VehicleKindIdQueryFilterArgument(kindId),
            new VehicleBrandIdQueryFilterArgument(brandId),
            new VehicleModelIdQueryFilterArgument(modelId),
            new VehiclePaginationQueryFilterArgument(page),
            RegionId: new VehicleRegionIdQueryFilterArgument(
                Guid.Parse("74487d05-0066-4528-8524-b3eca2b28624")
            )
        );
        IEnumerable<VehiclePresentation> vehicles = await connection.VehiclesOfCatalogue(
            logger,
            request,
            CancellationToken.None
        )();
        Assert.NotEmpty(vehicles);
    }

    [Fact]
    private async Task Query_Vehicles_With_Price_To_Success()
    {
        ILogger logger = new LoggerSource().Logger();
        await using PgConnectionSource source = new PgConnectionSource(fixture.DbConfig());
        await using NpgsqlConnection connection = await source.Connect();
        IEnumerable<VehicleKindPresentation> kinds = await connection.Provide(
            VehicleKindsPresentationSource.VehicleKindsCommand,
            VehicleKindsPresentationSource.VehicleKindsReader,
            VehicleKindsPresentationSource.VehicleKindsReading,
            CancellationToken.None
        );
        VehicleKindPresentation firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresentation> brands = await connection.Provide(
            kindId,
            VehicleBrandPresentationSource.CreateCommand,
            VehicleBrandPresentationSource.CreateReader,
            VehicleBrandPresentationSource.ProcessWithReader
        );
        VehicleBrandPresentation firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresentation> models = await connection.Provide(
            kindId,
            brandId,
            VehicleModelPresentationSource.VehicleModelsCommandSource,
            VehicleModelPresentationSource.VehicleModelsReaderSource,
            VehicleModelPresentationSource.VehicleModelsReadingSource
        );
        Guid modelId = models.First().Id;
        int page = 1;
        var request = new VehiclesQueryRequest(
            new VehicleKindIdQueryFilterArgument(kindId),
            new VehicleBrandIdQueryFilterArgument(brandId),
            new VehicleModelIdQueryFilterArgument(modelId),
            new VehiclePaginationQueryFilterArgument(page),
            Price: new VehiclePriceQueryFilterArgument(true, 1684900)
        );
        IEnumerable<VehiclePresentation> vehicles = await connection.VehiclesOfCatalogue(
            logger,
            request,
            CancellationToken.None
        )();
        Assert.NotEmpty(vehicles);
    }

    [Fact]
    private async Task Query_Vehicles_With_Price_From_Success()
    {
        ILogger logger = new LoggerSource().Logger();
        await using PgConnectionSource source = new PgConnectionSource(fixture.DbConfig());
        await using NpgsqlConnection connection = await source.Connect();
        IEnumerable<VehicleKindPresentation> kinds = await connection.Provide(
            VehicleKindsPresentationSource.VehicleKindsCommand,
            VehicleKindsPresentationSource.VehicleKindsReader,
            VehicleKindsPresentationSource.VehicleKindsReading,
            CancellationToken.None
        );
        VehicleKindPresentation firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresentation> brands = await connection.Provide(
            kindId,
            VehicleBrandPresentationSource.CreateCommand,
            VehicleBrandPresentationSource.CreateReader,
            VehicleBrandPresentationSource.ProcessWithReader
        );
        VehicleBrandPresentation firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresentation> models = await connection.Provide(
            kindId,
            brandId,
            VehicleModelPresentationSource.VehicleModelsCommandSource,
            VehicleModelPresentationSource.VehicleModelsReaderSource,
            VehicleModelPresentationSource.VehicleModelsReadingSource
        );
        Guid modelId = models.First().Id;
        int page = 1;
        var request = new VehiclesQueryRequest(
            new VehicleKindIdQueryFilterArgument(kindId),
            new VehicleBrandIdQueryFilterArgument(brandId),
            new VehicleModelIdQueryFilterArgument(modelId),
            new VehiclePaginationQueryFilterArgument(page),
            Price: new VehiclePriceQueryFilterArgument(PriceFrom: 1648900)
        );
        IEnumerable<VehiclePresentation> vehicles = await connection.VehiclesOfCatalogue(
            logger,
            request,
            CancellationToken.None
        )();
        Assert.NotEmpty(vehicles);
    }

    [Fact]
    private async Task Query_Vehicles_With_Price_Range_Success()
    {
        ILogger logger = new LoggerSource().Logger();
        await using PgConnectionSource source = new PgConnectionSource(fixture.DbConfig());
        await using NpgsqlConnection connection = await source.Connect();
        IEnumerable<VehicleKindPresentation> kinds = await connection.Provide(
            VehicleKindsPresentationSource.VehicleKindsCommand,
            VehicleKindsPresentationSource.VehicleKindsReader,
            VehicleKindsPresentationSource.VehicleKindsReading,
            CancellationToken.None
        );
        VehicleKindPresentation firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresentation> brands = await connection.Provide(
            kindId,
            VehicleBrandPresentationSource.CreateCommand,
            VehicleBrandPresentationSource.CreateReader,
            VehicleBrandPresentationSource.ProcessWithReader
        );
        VehicleBrandPresentation firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresentation> models = await connection.Provide(
            kindId,
            brandId,
            VehicleModelPresentationSource.VehicleModelsCommandSource,
            VehicleModelPresentationSource.VehicleModelsReaderSource,
            VehicleModelPresentationSource.VehicleModelsReadingSource
        );
        Guid modelId = models.First().Id;
        int page = 1;
        var request = new VehiclesQueryRequest(
            new VehicleKindIdQueryFilterArgument(kindId),
            new VehicleBrandIdQueryFilterArgument(brandId),
            new VehicleModelIdQueryFilterArgument(modelId),
            new VehiclePaginationQueryFilterArgument(page),
            Price: new VehiclePriceQueryFilterArgument(PriceFrom: 1648900, PriceTo: 1684900)
        );
        IEnumerable<VehiclePresentation> vehicles = await connection.VehiclesOfCatalogue(
            logger,
            request,
            CancellationToken.None
        )();
        Assert.NotEmpty(vehicles);
    }

    [Fact]
    private async Task Query_Vehicle_With_Characteristics_Success()
    {
        ILogger logger = new LoggerSource().Logger();
        await using PgConnectionSource source = new PgConnectionSource(fixture.DbConfig());
        await using NpgsqlConnection connection = await source.Connect();
        IEnumerable<VehicleKindPresentation> kinds = await connection.Provide(
            VehicleKindsPresentationSource.VehicleKindsCommand,
            VehicleKindsPresentationSource.VehicleKindsReader,
            VehicleKindsPresentationSource.VehicleKindsReading,
            CancellationToken.None
        );
        VehicleKindPresentation firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresentation> brands = await connection.Provide(
            kindId,
            VehicleBrandPresentationSource.CreateCommand,
            VehicleBrandPresentationSource.CreateReader,
            VehicleBrandPresentationSource.ProcessWithReader
        );
        VehicleBrandPresentation firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresentation> models = await connection.Provide(
            kindId,
            brandId,
            VehicleModelPresentationSource.VehicleModelsCommandSource,
            VehicleModelPresentationSource.VehicleModelsReaderSource,
            VehicleModelPresentationSource.VehicleModelsReadingSource
        );
        Guid modelId = models.First().Id;
        int page = 1;
        var request = new VehiclesQueryRequest(
            new VehicleKindIdQueryFilterArgument(kindId),
            new VehicleBrandIdQueryFilterArgument(brandId),
            new VehicleModelIdQueryFilterArgument(modelId),
            new VehiclePaginationQueryFilterArgument(page),
            Characteristics: new VehicleCharacteristicsQueryArguments(
                [
                    new VehicleCharacteristicQueryArgument(
                        Guid.Parse("9005d171-baf2-45af-a00e-e07a427968c4"),
                        "Высота выгрузки",
                        "3000"
                    ),
                ]
            )
        );
        IEnumerable<VehiclePresentation> vehicles = await connection.VehiclesOfCatalogue(
            logger,
            request,
            CancellationToken.None
        )();
        Assert.NotEmpty(vehicles);
    }
}
