namespace Parsing.Vehicles.Grpc.Recognition.LoadingWeight;

public sealed class LoadingWeightRecognition(ICommunicationChannel channel)
    : ILoadingWeightRecognition
{
    private readonly string _ctxKey = string.Intern("LOADING_WEIGHT");
    private readonly string _ctxName = string.Intern("Грузоподъёмность");

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await channel.Talker().Tell(text)).ByKeyOrDefault(
            _ctxKey,
            _ctxName
        );
    }
}
