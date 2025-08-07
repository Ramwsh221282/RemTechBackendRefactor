namespace Parsing.Vehicles.Grpc.Recognition.Region;

public sealed class RegionRecognition(ICommunicationChannel channel) : ICharacteristicRecognition
{
    private readonly string _ctxKey = VehicleConstants.REGION;
    private readonly string _ctxName = "Регион";

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await channel.Talker().Tell(text)).ByKeyOrDefault(
            _ctxKey,
            _ctxName
        );
    }
}
