namespace Parsing.Vehicles.Grpc.Recognition.BuRecognition;

public sealed class BuRecognition : IBuRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = string.Intern("BU");
    private readonly string _ctxName = string.Intern("Б/у");

    public BuRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }

    public async Task<Characteristic> Recognize(string text)
    {
        Characteristic characteristic = new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxName);
        return characteristic ? new Characteristic(_ctxName, string.Intern("Да")) : new Characteristic(_ctxName, string.Intern("Нет"));
    }
}