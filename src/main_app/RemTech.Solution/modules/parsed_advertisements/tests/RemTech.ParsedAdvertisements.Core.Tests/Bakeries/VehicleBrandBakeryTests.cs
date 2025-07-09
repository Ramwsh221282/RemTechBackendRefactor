using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Bakery;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Bakery.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Bakery.Receipts;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Bakery.Receipts.Decorators;
using RemTech.ParsedAdvertisements.Core.Tests.Moks;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Tests.Bakeries;

public sealed class VehicleBrandBakeryTests
{
    [Fact]
    private void Test_Bake_Vehicle_Brand_Success()
    {
        string text = "Ponsse";
        Guid id = Guid.NewGuid();
        Status<IVehicleBrand> brand = new VehicleDomainBaker().Baked(
            new LoggingVehicleBrandBakery(
                new MokLogger(),
                new CachingVehicleBrandBakery(
                    new ValidatingVehicleBrandBakery(new VehicleBrandBakery())
                )
            ),
            new FormattingVehicleBrandReceipt(
                new VehicleBrandReceipt(new NotEmptyString(text), new NotEmptyGuid(id))
            )
        );
        Assert.True(brand.IsSuccess);
        Assert.Equal(text, brand.Value.Identify().ReadText());
        Assert.Equal(id, brand.Value.Identify().ReadId());
    }
}
