namespace Parsing.Vehicles.Grpc.Recognition.LoadingHeight;

public sealed class LoadingHeightRecognition(ICommunicationChannel channel)
    : ILoadingHeightRecognition
{
    private readonly string _ctxKey = string.Intern("LOADING_HEIGHT");
    private readonly string _ctxName = string.Intern("Высота подъёма");

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await channel.Talker().Tell(text)).ByKeyOrDefault(
            _ctxKey,
            _ctxName
        );
    }
}
