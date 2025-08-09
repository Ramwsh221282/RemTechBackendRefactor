using Drom.Parsing.Vehicles.Parsing.Models;

namespace Drom.Parsing.Vehicles.Parsing.Logging;

public sealed class CarCharacteristicsCollectionLogMessage
{
    private readonly Queue<CarCharacteristicLogMessage> _messages;

    public CarCharacteristicsCollectionLogMessage()
    {
        _messages = [];
    }

    public void With(CarCharacteristic characteristic)
    {
        _messages.Enqueue(characteristic.LogMessage());
    }

    public void Log(Serilog.ILogger logger)
    {
        while (_messages.Count > 0)
        {
            CarCharacteristicLogMessage message = _messages.Dequeue();
            message.Log(logger);
        }
    }
}
