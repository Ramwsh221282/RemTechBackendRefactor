using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Decorators;
using RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Characteristics;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Tests.Vehicles.Characteristics;

public sealed class CharacteristicTests : IClassFixture<DataSourceTestsFixture>
{
    private readonly DataSourceTestsFixture _fixture;

    public CharacteristicTests(DataSourceTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private async Task Add_Characteristic_Success()
    {
        string name = "Мощность";
        CharacteristicEnvelope ctx = new NewCharacteristic(name);
        await using IAsyncCharacteristics characteristics = _fixture.Characteristics();
        Status<ICharacteristic> characteristic = await characteristics.Add(ctx);
        Assert.True(characteristic.IsSuccess);
    }

    [Fact]
    private async Task Add_Characteristic_Failure()
    {
        string name = string.Empty;
        CharacteristicEnvelope ctx = new NewCharacteristic(name);
        await using IAsyncCharacteristics characteristics = _fixture.Characteristics();
        Status<ICharacteristic> characteristic = await characteristics.Add(ctx);
        Assert.False(characteristic.IsSuccess);
    }
}
