using Drom.Parsing.Vehicles.Parsing.Logging;
using Parsing.RabbitMq.PublishVehicle;

namespace Drom.Parsing.Vehicles.Parsing.Models;

public sealed class CarCharacteristic(string name, string value)
{
    public CarCharacteristicLogMessage LogMessage()
    {
        return new CarCharacteristicLogMessage(name, value);
    }

    public void Print(LinkedList<VehicleBodyCharacteristic> list)
    {
        list.AddFirst(new VehicleBodyCharacteristic(name, value));
    }
}
