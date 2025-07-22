namespace Parsing.Vehicles.Grpc.Recognition.VehicleKind;

public sealed class VehicleKindRecognition : IVehicleKindRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = string.Intern("TRANSPORT_TYPE");
    private readonly string _ctxName = string.Intern("Тип техники");

    public VehicleKindRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }
    
    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxName);
    }
}