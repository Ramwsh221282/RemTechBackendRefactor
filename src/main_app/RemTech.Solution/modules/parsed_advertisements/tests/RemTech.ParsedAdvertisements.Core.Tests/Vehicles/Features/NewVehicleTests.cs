using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Ports;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.AddingNewVehicle;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.AddingNewVehicle.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Ports;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Ports;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Ports;
using RemTech.ParsedAdvertisements.Core.Tests.Moks;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Tests.Vehicles.Features;

public sealed class NewVehicleTests
{
    [Fact]
    private void Add_New_Vehicle_Success()
    {
        MokLogger logger = new();
        IVehicleKinds kinds = new LoggingVehicleKinds(
            logger,
            new ValidatingVehicleKinds(new DictionariedVehicleKinds())
        );
        IVehicleBrands brands = new LoggingVehicleBrands(
            logger,
            new ValidatingVehicleBrands(new DictionariedVehicleBrands())
        );
        IGeoLocations locations = new LoggingGeoLocations(
            logger,
            new ValidatingGeoLocations(new DictionariedGeoLocations())
        );
        ICharacteristics characteristics = new LoggingCharacteristics(
            logger,
            new ValidatingCharacteristics(new DictionariedCharacteristics())
        );
        IVehicles vehicles = new LoggingVehicles(
            logger,
            new ValidatingVehicles(new ListedVehicles())
        );
        string expectedKindName = "Погрузчик";
        string expectedBrandName = "Bulls";
        string expectedLocation = "Москва";
        string expectedLocationKind = "Область";
        long expectedPrice = 1000;
        bool isNds = true;
        string expectedId = "p321321";
        string expectedTitle = "Bulls погрузчик";
        string expectedDescription = "Желтый погрузчик";
        int expectedPhotosAmount = 1;
        int expectedCtxAmount = 1;
        Status<VehicleEnvelope> added = new LoggingAddedVehicle(
            logger,
            new KindedAddedVehicle(
                kinds,
                expectedKindName,
                new BrandedAddedVehicle(
                    brands,
                    expectedBrandName,
                    new LocationedAddedVehicle(
                        locations,
                        expectedLocation,
                        expectedLocationKind,
                        new PricedAddedVehicle(
                            expectedPrice,
                            "НДС",
                            new IdentifiedAddedVehicle(
                                expectedId,
                                new TextedAddedVehicle(
                                    expectedTitle,
                                    expectedDescription,
                                    new PhotographedAddedVehicle(
                                        ["http://localhost"],
                                        new CharacterizedAddedVehicle(
                                            [("двигатель", "3200 кВат")],
                                            characteristics,
                                            new AddedVehicle(vehicles)
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            )
        ).Added(new AddVehicle(new VehicleBlueprint()));
        Assert.True(added.IsSuccess);

        VehicleEnvelope value = added.Value;
        Assert.Equal(expectedKindName, value.Kind().Identify().ReadText());
        Assert.Equal(expectedBrandName, value.Brand().Identify().ReadText());
        Assert.Equal(expectedLocation, value.Location().Identify().ReadText());
        Assert.Equal(expectedPrice, value.Cost().Value());
        Assert.Equal(isNds, value.Cost().UnderNds());
        Assert.Equal(expectedPhotosAmount, value.Photos().Amount());
        Assert.Equal(expectedCtxAmount, value.Characteristics().Amount());
        Assert.Equal(expectedTitle, value.TextInformation().ReadTitle());
        Assert.Equal(expectedDescription, value.TextInformation().ReadDescription());
    }
}
