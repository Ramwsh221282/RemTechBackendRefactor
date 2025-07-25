namespace Parsing.Vehicles.Grpc.Recognition.FuelTankCapacity;

public sealed class FuelTankCapacityRecognition : IFuelTankCapacityRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = string.Intern("FUEL_TANK_CAPACITY");
    private readonly string _ctxName = string.Intern("Объём топливного бака");

    public FuelTankCapacityRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxName);
    }
}