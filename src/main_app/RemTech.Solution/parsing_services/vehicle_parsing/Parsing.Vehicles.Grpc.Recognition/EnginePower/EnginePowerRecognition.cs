namespace Parsing.Vehicles.Grpc.Recognition.EnginePower;

public sealed class EnginePowerRecognition : IEnginePowerRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = string.Intern("ENGINE_POWER");
    private readonly string _ctxName = string.Intern("Мощность двигателя");

    public EnginePowerRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxName);
    }
}