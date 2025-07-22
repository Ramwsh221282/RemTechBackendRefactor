namespace Parsing.Vehicles.Grpc.Recognition.VehicleBrand;

public sealed class VehicleBrandRecognition : IVehicleBrandRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = string.Intern("BRAND_NAME");
    private readonly string _ctxName = string.Intern("Марка");

    public VehicleBrandRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }
    
    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxName);
    }
}