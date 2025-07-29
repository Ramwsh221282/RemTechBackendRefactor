using RemTech.Core.Shared.Exceptions;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Tests.Fixtures;
using RemTech.ParsedAdvertisements.Core.Types.Brands;
using RemTech.ParsedAdvertisements.Core.Types.Brands.Decorators.Logic;
using RemTech.ParsedAdvertisements.Core.Types.Brands.Decorators.Postgres;
using RemTech.ParsedAdvertisements.Core.Types.Brands.Decorators.Validation;
using RemTech.ParsedAdvertisements.Core.Types.Brands.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Types.Characteristics;
using RemTech.ParsedAdvertisements.Core.Types.Characteristics.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Types.Characteristics.Features.Structuring;
using RemTech.ParsedAdvertisements.Core.Types.Characteristics.Ports.Storage;
using RemTech.ParsedAdvertisements.Core.Types.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Types.GeoLocations.Decorators.Logic;
using RemTech.ParsedAdvertisements.Core.Types.GeoLocations.Decorators.Postgres;
using RemTech.ParsedAdvertisements.Core.Types.GeoLocations.Decorators.Validation;
using RemTech.ParsedAdvertisements.Core.Types.GeoLocations.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Types.Kinds;
using RemTech.ParsedAdvertisements.Core.Types.Kinds.Decorators.Logic;
using RemTech.ParsedAdvertisements.Core.Types.Kinds.Decorators.Postgres;
using RemTech.ParsedAdvertisements.Core.Types.Kinds.Decorators.Validation;
using RemTech.ParsedAdvertisements.Core.Types.Kinds.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Types.Models;
using RemTech.ParsedAdvertisements.Core.Types.Models.Decorators.Logic;
using RemTech.ParsedAdvertisements.Core.Types.Models.Decorators.Postgres;
using RemTech.ParsedAdvertisements.Core.Types.Models.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Types.Transport;
using RemTech.ParsedAdvertisements.Core.Types.Transport.Decorators;
using RemTech.ParsedAdvertisements.Core.Types.Transport.Decorators.Postgres;
using RemTech.ParsedAdvertisements.Core.Types.Transport.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Types.Transport.ValueObjects.Prices;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Tests.Vehicles.Features;

public sealed class SinkVehiclesTests(PgTestsFixture fixture) : IClassFixture<PgTestsFixture>
{
    [Fact]
    private void Create_Vehicle_Failure()
    {
        IItemPrice price = new ItemPriceWithNds(new PriceValue(10000));
        VehicleIdentity identity = new VehicleIdentity(new VehicleId("A123123231312321"));
        VehiclePhotos photos = new VehiclePhotos([new VehiclePhoto("http://localhost")]);
        Assert.ThrowsAny<ValueNotValidException>(() =>
            new ValidVehicle(new Vehicle(identity, price, photos))
        );
    }

    [Fact]
    private async Task Save_Vehicle_Success()
    {
        await using PgConnectionSource connectionSource = new(fixture.DbConfig());
        IPgCharacteristicsStorage ctxes = new PgVarietCharacteristicsStorage()
            .With(new PgCharacteristicsStorage(connectionSource))
            .With(new PgDuplicateResolvingCharacteristicsStorage(connectionSource));
        VehicleBrand brand = await new PgVarietVehicleBrand(
            connectionSource,
            new ValidVehicleBrand(
                new VehicleBrand(
                    new VehicleBrandIdentity(
                        new VehicleBrandId(Guid.NewGuid()),
                        new VehicleBrandText("Ponsse")
                    )
                )
            )
        ).SaveAsync(CancellationToken.None);
        VehicleKind kind = await new PgVarietVehicleKind(
            connectionSource,
            new ValidVehicleKind(
                new VehicleKind(
                    new VehicleKindIdentity(
                        new VehicleKindId(Guid.NewGuid()),
                        new VehicleKindText("Форвардер")
                    )
                )
            )
        ).SaveAsync(CancellationToken.None);
        GeoLocation location = await new PgGeoLocation(
            connectionSource,
            new ValidGeoLocation(
                new GeoLocation(
                    new GeoLocationIdentity(
                        new GeoLocationId(new NotEmptyGuid(Guid.NewGuid())),
                        new GeolocationText(new NotEmptyString("Красноярский")),
                        new GeolocationText(new NotEmptyString("край"))
                    )
                )
            )
        ).SaveAsync(CancellationToken.None);

        VehicleModel model = await new PgVarietVehicleModel(
            connectionSource,
            new VehicleModel(
                new VehicleModelIdentity(Guid.NewGuid()),
                new VehicleModelName(new NotEmptyString("Buffalo"))
            )
        ).SaveAsync(CancellationToken.None);
        CharacteristicVeil unstructured = new CharacteristicVeil(
            new NotEmptyString("Грузоподъёмность"),
            new NotEmptyString("3300 кг")
        );
        Characteristic stored = await ctxes.Stored(unstructured.Characteristic());
        IItemPrice price = new ItemPriceWithNds(new PriceValue(10000));
        VehicleIdentity identity = new VehicleIdentity(new VehicleId("A123123231312321"));
        VehiclePhotos photos = new VehiclePhotos([new VehiclePhoto("http://localhost")]);
        Vehicle vehicle = new ValidVehicle(
            stored.ToVehicle(
                new ModelingVehicleModel(model).ModeledVehicle(
                    new LocationingGeoLocation(location).Locatate(
                        new KindingVehicleKind(kind).KindVehicle(
                            new BrandingVehicleBrand(brand).BrandVehicle(
                                new Vehicle(identity, price, photos)
                            )
                        )
                    )
                )
            )
        );
        await new PgTransactionalVehicle(connectionSource, vehicle).SaveAsync(
            CancellationToken.None
        );
    }
}
