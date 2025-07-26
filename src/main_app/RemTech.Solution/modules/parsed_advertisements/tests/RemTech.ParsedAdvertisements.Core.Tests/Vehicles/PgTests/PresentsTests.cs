using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehicleBrandPresentation;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehicleKindsPresentation;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehicleModelsPresentation;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.QueryModifiers;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.QueryParameters;
using RemTech.ParsedAdvertisements.Core.Tests.Fixtures;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Tests.Vehicles.PgTests;

public sealed class PresentsTests(PgTestsFixture fixture) : IClassFixture<PgTestsFixture>
{
    [Fact]
    private async Task Read_Vehicles()
    {
        await using PgConnectionSource connectionSource = new PgConnectionSource(fixture.DbConfig());
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
        IVehiclePresentQueryMod[] mods =
        [
            new KindedVehiclePresentMod(kindId),
            new BrandedVehiclePresentMod(brandId),
            new ModeledVehiclePresentMod(modelId)
        ];
        IEnumerable<VehiclePresent> presents = await new VehiclePresentsSource(connectionSource, mods).Read();
        Assert.NotEmpty(presents);
    }

    [Fact]
    private async Task Read_Vehicles_With_Price_Filtering()
    {
        await using PgConnectionSource connectionSource = new PgConnectionSource(fixture.DbConfig());
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
        IVehiclePresentQueryMod[] mods =
        [
            new KindedVehiclePresentMod(kindId),
            new BrandedVehiclePresentMod(brandId),
            new ModeledVehiclePresentMod(modelId),
            new PriceRangeQueryMod(1600000, 1650000)
        ];
        IEnumerable<VehiclePresent> presents = await new VehiclePresentsSource(connectionSource, mods).Read();
        Assert.NotEmpty(presents);
    }

    [Fact]
    private async Task Read_Vehicles_With_Ctx_Filtering()
    {
        await using PgConnectionSource connectionSource = new PgConnectionSource(fixture.DbConfig());
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
        IVehiclePresentQueryMod[] mods =
        [
            new KindedVehiclePresentMod(kindId),
            new BrandedVehiclePresentMod(brandId),
            new ModeledVehiclePresentMod(modelId),
            new CharacteristicsQueryMod(
            [
                new CharacteristicsQueryModParameter(Guid.Parse("e362f179-3fa6-4b04-8188-b122c451deeb"), "Грузоподъёмность", "2000"),
                new CharacteristicsQueryModParameter(Guid.Parse("c72686a9-59f0-49dd-8412-d61ac1a62b39"), "Высота выгрузки", "3000"),
            ])
        ];
        IEnumerable<VehiclePresent> presents = await new VehiclePresentsSource(connectionSource, mods).Read();
        Assert.NotEmpty(presents);
    }

    [Fact]
    private async Task Read_Vehicles_Multiple_Filters()
    {
        await using PgConnectionSource connectionSource = new PgConnectionSource(fixture.DbConfig());
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
        IVehiclePresentQueryMod[] mods =
        [
            new KindedVehiclePresentMod(kindId),
            new BrandedVehiclePresentMod(brandId),
            new ModeledVehiclePresentMod(modelId),
            new PriceRangeQueryMod(1600000, 1650000),
            new CharacteristicsQueryMod(
            [
                new CharacteristicsQueryModParameter(Guid.Parse("e362f179-3fa6-4b04-8188-b122c451deeb"), "Грузоподъёмность", "2000"),
                new CharacteristicsQueryModParameter(Guid.Parse("c72686a9-59f0-49dd-8412-d61ac1a62b39"), "Высота выгрузки", "3000"),
            ])
        ];
        IEnumerable<VehiclePresent> presents = await new VehiclePresentsSource(connectionSource, mods).Read();
        Assert.NotEmpty(presents);
    }
}