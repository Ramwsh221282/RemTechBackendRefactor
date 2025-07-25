namespace Parsing.Vehicles.Grpc.Recognition.Region;

public sealed class RegionRecognition : ICharacteristicRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = VehicleConstants.REGION;
    private readonly string _ctxName = "Регион";

    public RegionRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }
    
    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxName);
    }
}