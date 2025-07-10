using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;

namespace RemTech.ParsedAdvertisements.Core.Tests.Vehicles;

public sealed class VehicleEnvelopeTests
{
    [Fact]
    private void Vehicle_With_Text_Success()
    {
        string title = "Погрузчик";
        string description = "Желтый погрузчик";
        VehicleEnvelope vehicle = new TextFormattedVehicle(
            title,
            description,
            new VehicleBlueprint()
        );
        Assert.Equal(title, vehicle.TextInformation().ReadTitle());
        Assert.Equal(description, vehicle.TextInformation().ReadDescription());
    }

    [Fact]
    private void Vehicle_With_Kind_And_Text_Success()
    {
        string kindName = "Железный";
        string title = "Погрузчик";
        string description = "Желтый погрузчик";
        VehicleEnvelope vehicle = new TextFormattedVehicle(
            title,
            description,
            new KindedVehicle(kindName)
        );
        Assert.Equal(title, vehicle.TextInformation().ReadTitle());
        Assert.Equal(description, vehicle.TextInformation().ReadDescription());
        Assert.Equal(kindName, vehicle.Kind().Identify().ReadText());
    }
}
