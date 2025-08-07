namespace Parsing.Vehicles.Grpc.Recognition.FuelTankCapacity;

public sealed class FuelTankCapacityRecognition(ICommunicationChannel channel)
    : IFuelTankCapacityRecognition
{
    private readonly string _ctxKey = string.Intern("FUEL_TANK_CAPACITY");
    private readonly string _ctxName = string.Intern("Объём топливного бака");

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await channel.Talker().Tell(text)).ByKeyOrDefault(
            _ctxKey,
            _ctxName
        );
    }
}
