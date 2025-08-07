namespace Parsing.Vehicles.Grpc.Recognition.Weight;

public sealed class WeightRecognition(ICommunicationChannel channel) : IWeightRecognition
{
    private readonly string _ctxKey = string.Intern("WEIGHT");
    private readonly string _ctxName = string.Intern("Эксплуатационная масса");

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await channel.Talker().Tell(text)).ByKeyOrDefault(
            _ctxKey,
            _ctxName
        );
    }
}
