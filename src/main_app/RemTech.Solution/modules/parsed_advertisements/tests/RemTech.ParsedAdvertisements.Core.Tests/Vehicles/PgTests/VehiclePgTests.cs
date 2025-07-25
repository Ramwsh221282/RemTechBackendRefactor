using RemTech.Core.Shared.Exceptions;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Ports.Storage;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports.Storage;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Ports.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Ports.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Ports.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Tests.Fixtures;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Tests.Vehicles.PgTests;

public sealed class VehiclePgTests(PgTestsFixture fixture) : IClassFixture<PgTestsFixture>
{
    [Fact]
    private void Create_Vehicle_Failure()
    {
        IItemPrice price = new ItemPriceWithNds(new PriceValue(10000));
        VehicleIdentity identity = new VehicleIdentity(new VehicleId("A123123231312321"));
        VehiclePhotos photos = new VehiclePhotos([new VehiclePhoto("http://localhost")]);
        Assert.ThrowsAny<ValueNotValidException>(() => new LoggingVehicle(fixture.Logger(),
            new ValidVehicle(
                new Vehicle(identity, price, photos))));
    }

    [Fact]
    private async Task Save_Vehicle_Success()
    {
        await using PgConnectionSource connectionSource = new(fixture.DbConfig());
        IPgVehicleBrandsStorage brands = new PgVarietVehicleBrandsStorage()
            .With(new PgLoggingVehicleBrandsStorage(fixture.Logger(), 
                new PgVehicleBrandsStore(connectionSource)))
            .With(new PgLoggingVehicleBrandsStorage(fixture.Logger(), 
                new PgDuplicateResolvingVehicleBrandsStore(connectionSource)));
        IPgVehicleKindsStorage kinds = new PgVarietVehicleKindStorage()
            .With(new PgLoggingVehicleKindStorage(fixture.Logger(), new PgVehicleKindsStorage(connectionSource)))
            .With(new PgLoggingVehicleKindStorage(fixture.Logger(),
                new PgDuplicateResolvingVehicleKindStorage(connectionSource)));
        IPgVehicleGeosStorage locations = new PgVarietVehicleGeosStorage()
            .With(new PgLoggingVehicleGeosStorage(fixture.Logger(), new PgVehicleGeosStorage(connectionSource)));
        IPgVehicleModelsStorage models = new PgVarietVehicleModelsStorage()
            .With(new PgLoggingVehicleModelsStorage(fixture.Logger(), new PgVehicleModelsStorage(connectionSource)))
            .With(new PgLoggingVehicleModelsStorage(fixture.Logger(), new PgDuplicateResolvingVehicleModelsStorage(connectionSource)));
        IPgCharacteristicsStorage ctxes = new PgVarietCharacteristicsStorage()
            .With(new PgLoggingCharacteristicsStorage(fixture.Logger(), new PgCharacteristicsStorage(connectionSource)))
            .With(new PgLoggingCharacteristicsStorage(fixture.Logger(),
                new PgDuplicateResolvingCharacteristicsStorage(connectionSource)));
        VehicleBrand brand = await brands.Get(new LoggingVehicleBrand(
            fixture.Logger(),
            new ValidVehicleBrand(
                new VehicleBrand(
                    new VehicleBrandIdentity(new VehicleBrandId(Guid.NewGuid()), new VehicleBrandText("Ponsse"))
                ))), CancellationToken.None);
        VehicleKind kind = await kinds.Read(new LoggingVehicleKind(
                fixture.Logger(),
                new ValidVehicleKind(
                    new VehicleKind(
                        new VehicleKindIdentity(new VehicleKindId(Guid.NewGuid()), new VehicleKindText("Форвардер"))))),
            CancellationToken.None);
        GeoLocation location = await locations.Get(new LoggingGeoLocation(fixture.Logger(),
            new ValidGeoLocation(
                new GeoLocation(
                    new GeoLocationIdentity(
                        new GeoLocationId(new NotEmptyGuid(Guid.NewGuid())),
                        new GeolocationText(new NotEmptyString("Красноярский")),
                        new GeolocationText(new NotEmptyString("край")))))), CancellationToken.None);
        VehicleModel model = await models.Get(
            new VehicleModel(
                new VehicleModelIdentity(Guid.NewGuid()),
                new VehicleModelName(new NotEmptyString("Buffalo"))), CancellationToken.None);
        UnstructuredCharacteristic unstructured = new UnstructuredCharacteristic(
            new NotEmptyString("Грузоподъёмность"),
            new NotEmptyString("3300 кг"));
        ICharacteristic stored = await ctxes.Stored(unstructured);
        IItemPrice price = new ItemPriceWithNds(new PriceValue(10000));
        VehicleIdentity identity = new VehicleIdentity(new VehicleId("A123123231312321"));
        VehiclePhotos photos = new VehiclePhotos([new VehiclePhoto("http://localhost")]);
        Vehicle vehicle = unstructured.TryPut(model.Print(
            location.Print(
                kind.Print(
                    brand.Print(
                        new Vehicle(identity, price, photos))))));
        vehicle = new LoggingVehicle(fixture.Logger(), new ValidVehicle(vehicle));
        await new PgTransactionalVehicle(connectionSource, vehicle).SaveAsync(CancellationToken.None);
    }
}