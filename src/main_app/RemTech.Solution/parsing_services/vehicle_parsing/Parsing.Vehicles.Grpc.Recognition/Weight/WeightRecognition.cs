namespace Parsing.Vehicles.Grpc.Recognition.Weight;

public sealed class WeightRecognition : IWeightRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = string.Intern("WEIGHT");
    private readonly string _ctxName = string.Intern("Эксплуатационная масса");

    public WeightRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxName);
    }
}