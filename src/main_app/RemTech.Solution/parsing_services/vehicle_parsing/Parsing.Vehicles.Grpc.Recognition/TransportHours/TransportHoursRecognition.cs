namespace Parsing.Vehicles.Grpc.Recognition.TransportHours;

public sealed class TransportHoursRecognition : ITransportHoursRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = string.Intern("TRANSPORT_HOURS");
    private readonly string _ctxName = string.Intern("Моточасы");

    public TransportHoursRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }
    
    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxName);
    }
}