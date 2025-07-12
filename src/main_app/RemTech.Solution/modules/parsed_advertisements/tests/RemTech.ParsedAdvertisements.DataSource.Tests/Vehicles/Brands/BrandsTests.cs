using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators;
using RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Brands;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Tests.Vehicles.Brands;

public sealed class BrandsTests : IClassFixture<DataSourceTestsFixture>
{
    private readonly DataSourceTestsFixture _fixture;

    public BrandsTests(DataSourceTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private async Task Add_New_Brand_Success()
    {
        string brandName = "LuGong";
        VehicleBrandEnvelope brand = new NewVehicleBrand(brandName);
        await using IAsyncVehicleBrands brands = _fixture.VehicleBrands();
        Status<IVehicleBrand> added = await brands.Add(brand);
        Assert.True(added.IsSuccess);
        Assert.Equal(brandName, added.Value.Identify().ReadText());
    }

    [Fact]
    private async Task Add_New_Brand_Name_Failure()
    {
        string brandName = string.Empty;
        VehicleBrandEnvelope brand = new NewVehicleBrand(brandName);
        await using IAsyncVehicleBrands brands = _fixture.VehicleBrands();
        Status<IVehicleBrand> added = await brands.Add(brand);
        Assert.True(added.IsFailure);
    }

    [Fact]
    private async Task Add_Similar_Brand_Success()
    {
        string brandName = "LuGong";
        VehicleBrandEnvelope brand = new NewVehicleBrand(brandName);
        await using IAsyncVehicleBrands brands = _fixture.VehicleBrands();
        Status<IVehicleBrand> added = await brands.Add(brand);
        Assert.True(added.IsSuccess);
        Assert.Equal(brandName, added.Value.Identify().ReadText());
        string brandRelated1 = "Фронтальный погрузчик LuGong LM938, 2025";
        string brandRelated2 = "Мини-фронтальный погрузчик LUGONG T916";
        Status<IVehicleBrand> addedRelated1 = await brands.Add(new NewVehicleBrand(brandRelated1));
        Status<IVehicleBrand> addedRelated2 = await brands.Add(new NewVehicleBrand(brandRelated2));
        Assert.True(addedRelated1.IsSuccess);
        Assert.True(addedRelated2.IsSuccess);
        string related1Name = addedRelated1.Value.Identify().ReadText();
        string related2Name = addedRelated2.Value.Identify().ReadText();
        Assert.Equal(brandName, related1Name);
        Assert.Equal(brandName, related2Name);
    }
}
