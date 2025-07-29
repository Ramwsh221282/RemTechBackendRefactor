using Npgsql;
using RemTech.Logging.Adapter;
using RemTech.Postgres.Adapter.Library;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Arguments;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.Types;
using RemTech.Vehicles.Module.Features.VehicleBrandPresentation;
using RemTech.Vehicles.Module.Features.VehicleKindsPresentation;
using RemTech.Vehicles.Module.Features.VehicleModelsPresentation;
using RemTech.Vehicles.Module.Tests.Fixtures;
using Serilog;

namespace RemTech.Vehicles.Module.Tests.Vehicles.Features.QueryVehiclesCatalogueTests;

public sealed class QueryVehiclesCatalogueTests(PgTestsFixture fixture)
    : IClassFixture<PgTestsFixture>
{
    [Fact]
    private async Task Query_Vehicles_Catalogue_Standard_Success()
    {
        await using PgConnectionSource connectionSource = new PgConnectionSource(
            fixture.DbConfig()
        );
        ILogger logger = new LoggerSource().Logger();
        IEnumerable<VehicleKindPresent> kinds = await new VehicleKindPresentsSource(
            connectionSource
        ).ReadAsync();
        VehicleKindPresent firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresent> brands = await new VehicleBrandPresentsSource(
            kindId,
            connectionSource
        ).ReadAsync();
        VehicleBrandPresent firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresent> models = await new VehicleModelPresentsSource(
            brandId,
            kindId,
            connectionSource
        ).ReadAsync(CancellationToken.None);
        VehicleModelPresent firstModel = models.First();
        Guid modelId = firstModel.Id;
        int page = 1;
        var request = new VehiclesQueryRequest(
            new VehicleKindIdQueryFilterArgument(kindId),
            new VehicleBrandIdQueryFilterArgument(brandId),
            new VehicleModelIdQueryFilterArgument(modelId),
            new VehiclePaginationQueryFilterArgument(page)
        );
        await using NpgsqlConnection connection = await connectionSource.Connect(
            CancellationToken.None
        );
        VehiclesCataloguePresentation catalogue =
            await VehiclesCatalogueProvider.VehiclesCatalogue()(
                connection.VehiclesOfCatalogue(logger, request, CancellationToken.None),
                connection.CharacteristicsOfCatalogue(request, CancellationToken.None),
                connection.AggregatedDataOfCatalogue(request, CancellationToken.None)
            );
        Assert.NotEmpty(catalogue.Vehicles);
        Assert.NotEmpty(catalogue.Characteristics.Characteristics);
        Assert.True(catalogue.AggregatedData.TotalCount > 0);
        Assert.True(catalogue.AggregatedData.PagesCount > 0);
        Assert.True(catalogue.AggregatedData.MaximalPrice > 0);
        Assert.True(catalogue.AggregatedData.MinimalPrice > 0);
        Assert.True(catalogue.AggregatedData.AveragePrice > 0);
    }

    [Fact]
    private async Task Query_Vehicles_Catalogue_With_Location_Success()
    {
        await using PgConnectionSource connectionSource = new PgConnectionSource(
            fixture.DbConfig()
        );
        ILogger logger = new LoggerSource().Logger();
        IEnumerable<VehicleKindPresent> kinds = await new VehicleKindPresentsSource(
            connectionSource
        ).ReadAsync();
        VehicleKindPresent firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresent> brands = await new VehicleBrandPresentsSource(
            kindId,
            connectionSource
        ).ReadAsync();
        VehicleBrandPresent firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresent> models = await new VehicleModelPresentsSource(
            brandId,
            kindId,
            connectionSource
        ).ReadAsync(CancellationToken.None);
        VehicleModelPresent firstModel = models.First();
        Guid modelId = firstModel.Id;
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
        await using NpgsqlConnection connection = await connectionSource.Connect(
            CancellationToken.None
        );
        VehiclesCataloguePresentation catalogue =
            await VehiclesCatalogueProvider.VehiclesCatalogue()(
                connection.VehiclesOfCatalogue(logger, request, CancellationToken.None),
                connection.CharacteristicsOfCatalogue(request, CancellationToken.None),
                connection.AggregatedDataOfCatalogue(request, CancellationToken.None)
            );
        Assert.NotEmpty(catalogue.Vehicles);
        Assert.NotEmpty(catalogue.Characteristics.Characteristics);
        Assert.True(catalogue.AggregatedData.TotalCount > 0);
        Assert.True(catalogue.AggregatedData.PagesCount > 0);
        Assert.True(catalogue.AggregatedData.MaximalPrice > 0);
        Assert.True(catalogue.AggregatedData.MinimalPrice > 0);
        Assert.True(catalogue.AggregatedData.AveragePrice > 0);
    }

    [Fact]
    private async Task Query_Vehicles_Catalogue_With_Price_To_Success()
    {
        await using PgConnectionSource connectionSource = new PgConnectionSource(
            fixture.DbConfig()
        );
        ILogger logger = new LoggerSource().Logger();
        IEnumerable<VehicleKindPresent> kinds = await new VehicleKindPresentsSource(
            connectionSource
        ).ReadAsync();
        VehicleKindPresent firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresent> brands = await new VehicleBrandPresentsSource(
            kindId,
            connectionSource
        ).ReadAsync();
        VehicleBrandPresent firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresent> models = await new VehicleModelPresentsSource(
            brandId,
            kindId,
            connectionSource
        ).ReadAsync(CancellationToken.None);
        VehicleModelPresent firstModel = models.First();
        Guid modelId = firstModel.Id;
        int page = 1;
        var request = new VehiclesQueryRequest(
            new VehicleKindIdQueryFilterArgument(kindId),
            new VehicleBrandIdQueryFilterArgument(brandId),
            new VehicleModelIdQueryFilterArgument(modelId),
            new VehiclePaginationQueryFilterArgument(page),
            Price: new VehiclePriceQueryFilterArgument(true, 1684900)
        );
        await using NpgsqlConnection connection = await connectionSource.Connect(
            CancellationToken.None
        );
        VehiclesCataloguePresentation catalogue =
            await VehiclesCatalogueProvider.VehiclesCatalogue()(
                connection.VehiclesOfCatalogue(logger, request, CancellationToken.None),
                connection.CharacteristicsOfCatalogue(request, CancellationToken.None),
                connection.AggregatedDataOfCatalogue(request, CancellationToken.None)
            );
        Assert.NotEmpty(catalogue.Vehicles);
        Assert.NotEmpty(catalogue.Characteristics.Characteristics);
        Assert.True(catalogue.AggregatedData.TotalCount > 0);
        Assert.True(catalogue.AggregatedData.PagesCount > 0);
        Assert.True(catalogue.AggregatedData.MaximalPrice > 0);
        Assert.True(catalogue.AggregatedData.MinimalPrice > 0);
        Assert.True(catalogue.AggregatedData.AveragePrice > 0);
    }

    [Fact]
    private async Task Query_Vehicles_Catalogue_With_Price_From_Success()
    {
        await using PgConnectionSource connectionSource = new PgConnectionSource(
            fixture.DbConfig()
        );
        ILogger logger = new LoggerSource().Logger();
        IEnumerable<VehicleKindPresent> kinds = await new VehicleKindPresentsSource(
            connectionSource
        ).ReadAsync();
        VehicleKindPresent firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresent> brands = await new VehicleBrandPresentsSource(
            kindId,
            connectionSource
        ).ReadAsync();
        VehicleBrandPresent firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresent> models = await new VehicleModelPresentsSource(
            brandId,
            kindId,
            connectionSource
        ).ReadAsync(CancellationToken.None);
        VehicleModelPresent firstModel = models.First();
        Guid modelId = firstModel.Id;
        int page = 1;
        var request = new VehiclesQueryRequest(
            new VehicleKindIdQueryFilterArgument(kindId),
            new VehicleBrandIdQueryFilterArgument(brandId),
            new VehicleModelIdQueryFilterArgument(modelId),
            new VehiclePaginationQueryFilterArgument(page),
            Price: new VehiclePriceQueryFilterArgument(PriceFrom: 1648900)
        );
        await using NpgsqlConnection connection = await connectionSource.Connect(
            CancellationToken.None
        );
        VehiclesCataloguePresentation catalogue =
            await VehiclesCatalogueProvider.VehiclesCatalogue()(
                connection.VehiclesOfCatalogue(logger, request, CancellationToken.None),
                connection.CharacteristicsOfCatalogue(request, CancellationToken.None),
                connection.AggregatedDataOfCatalogue(request, CancellationToken.None)
            );
        Assert.NotEmpty(catalogue.Vehicles);
        Assert.NotEmpty(catalogue.Characteristics.Characteristics);
        Assert.True(catalogue.AggregatedData.TotalCount > 0);
        Assert.True(catalogue.AggregatedData.PagesCount > 0);
        Assert.True(catalogue.AggregatedData.MaximalPrice > 0);
        Assert.True(catalogue.AggregatedData.MinimalPrice > 0);
        Assert.True(catalogue.AggregatedData.AveragePrice > 0);
    }

    [Fact]
    private async Task Query_Vehicles_Catalogue_With_Price_Range_Success()
    {
        await using PgConnectionSource connectionSource = new PgConnectionSource(
            fixture.DbConfig()
        );
        ILogger logger = new LoggerSource().Logger();
        IEnumerable<VehicleKindPresent> kinds = await new VehicleKindPresentsSource(
            connectionSource
        ).ReadAsync();
        VehicleKindPresent firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresent> brands = await new VehicleBrandPresentsSource(
            kindId,
            connectionSource
        ).ReadAsync();
        VehicleBrandPresent firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresent> models = await new VehicleModelPresentsSource(
            brandId,
            kindId,
            connectionSource
        ).ReadAsync(CancellationToken.None);
        VehicleModelPresent firstModel = models.First();
        Guid modelId = firstModel.Id;
        int page = 1;
        var request = new VehiclesQueryRequest(
            new VehicleKindIdQueryFilterArgument(kindId),
            new VehicleBrandIdQueryFilterArgument(brandId),
            new VehicleModelIdQueryFilterArgument(modelId),
            new VehiclePaginationQueryFilterArgument(page),
            Price: new VehiclePriceQueryFilterArgument(PriceFrom: 1648900, PriceTo: 1684900)
        );
        await using NpgsqlConnection connection = await connectionSource.Connect(
            CancellationToken.None
        );
        VehiclesCataloguePresentation catalogue =
            await VehiclesCatalogueProvider.VehiclesCatalogue()(
                connection.VehiclesOfCatalogue(logger, request, CancellationToken.None),
                connection.CharacteristicsOfCatalogue(request, CancellationToken.None),
                connection.AggregatedDataOfCatalogue(request, CancellationToken.None)
            );
        Assert.NotEmpty(catalogue.Vehicles);
        Assert.NotEmpty(catalogue.Characteristics.Characteristics);
        Assert.True(catalogue.AggregatedData.TotalCount > 0);
        Assert.True(catalogue.AggregatedData.PagesCount > 0);
        Assert.True(catalogue.AggregatedData.MaximalPrice > 0);
        Assert.True(catalogue.AggregatedData.MinimalPrice > 0);
        Assert.True(catalogue.AggregatedData.AveragePrice > 0);
    }

    [Fact]
    private async Task Query_Vehicles_Catalogue_With_Characteristics_Success()
    {
        await using PgConnectionSource connectionSource = new PgConnectionSource(
            fixture.DbConfig()
        );
        ILogger logger = new LoggerSource().Logger();
        IEnumerable<VehicleKindPresent> kinds = await new VehicleKindPresentsSource(
            connectionSource
        ).ReadAsync();
        VehicleKindPresent firstKind = kinds.First();
        Guid kindId = firstKind.Id;
        IEnumerable<VehicleBrandPresent> brands = await new VehicleBrandPresentsSource(
            kindId,
            connectionSource
        ).ReadAsync();
        VehicleBrandPresent firstBrand = brands.First();
        Guid brandId = firstBrand.Id;
        IEnumerable<VehicleModelPresent> models = await new VehicleModelPresentsSource(
            brandId,
            kindId,
            connectionSource
        ).ReadAsync(CancellationToken.None);
        VehicleModelPresent firstModel = models.First();
        Guid modelId = firstModel.Id;
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
        await using NpgsqlConnection connection = await connectionSource.Connect(
            CancellationToken.None
        );
        VehiclesCataloguePresentation catalogue =
            await VehiclesCatalogueProvider.VehiclesCatalogue()(
                connection.VehiclesOfCatalogue(logger, request, CancellationToken.None),
                connection.CharacteristicsOfCatalogue(request, CancellationToken.None),
                connection.AggregatedDataOfCatalogue(request, CancellationToken.None)
            );
        Assert.NotEmpty(catalogue.Vehicles);
        Assert.NotEmpty(catalogue.Characteristics.Characteristics);
        Assert.True(catalogue.AggregatedData.TotalCount > 0);
        Assert.True(catalogue.AggregatedData.PagesCount > 0);
        Assert.True(catalogue.AggregatedData.MaximalPrice > 0);
        Assert.True(catalogue.AggregatedData.MinimalPrice > 0);
    }
}
