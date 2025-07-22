namespace Parsing.Vehicles.Grpc.Recognition.VehicleModel;

public sealed class VehicleModelRecognition : IVehicleModelRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = string.Intern("BRAND_MODEL");
    private readonly string _ctxName = string.Intern("Модель");

    public VehicleModelRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }
    
    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxName);
    }
}