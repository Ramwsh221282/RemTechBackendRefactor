namespace Parsing.Vehicles.Grpc.Recognition.Torque;

public sealed class TorqueRecognition : ITorqueRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = string.Intern("TORQUE");
    private readonly string _ctxName = string.Intern("Крутящий момент");

    public TorqueRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }
    
    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxName);
    }
}