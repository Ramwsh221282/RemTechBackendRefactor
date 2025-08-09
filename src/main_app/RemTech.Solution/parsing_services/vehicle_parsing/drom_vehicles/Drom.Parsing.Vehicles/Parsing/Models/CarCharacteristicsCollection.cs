using Drom.Parsing.Vehicles.Parsing.Logging;
using Parsing.RabbitMq.PublishVehicle;

namespace Drom.Parsing.Vehicles.Parsing.Models;

public sealed class CarCharacteristicsCollection
{
    private readonly Dictionary<string, CarCharacteristic> _characteristics = [];

    public void With(string name, string value)
    {
        if (_characteristics.ContainsKey(name))
            return;
        CarCharacteristic characteristic = new CarCharacteristic(name, value);
        _characteristics.Add(name, characteristic);
    }

    public int Amount() => _characteristics.Count;

    public void Print(DromCatalogueCar car)
    {
        car.WithCharacteristics(this);
    }

    public CarCharacteristicsCollectionLogMessage LogMessage()
    {
        CarCharacteristicsCollectionLogMessage message = new();
        foreach (CarCharacteristic characteristic in _characteristics.Values)
            message.With(characteristic);
        return message;
    }

    public VehiclePublishMessage PrintTo(VehiclePublishMessage message)
    {
        LinkedList<VehicleBodyCharacteristic> characteristics = [];
        foreach (CarCharacteristic characteristic in _characteristics.Values)
        {
            characteristic.Print(characteristics);
        }

        return message with
        {
            Vehicle = message.Vehicle with { Characteristics = characteristics },
        };
    }
}
