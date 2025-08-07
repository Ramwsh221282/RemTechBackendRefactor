namespace Parsing.Vehicles.Grpc.Recognition.UnloadingHeight;

public sealed class UnloadingHeightRecognition(ICommunicationChannel channel)
    : IUnloadingHeightRecognition
{
    private readonly string _ctxKey = string.Intern("UNLOADING_HEIGHT");
    private readonly string _ctxName = string.Intern("Высота выгрузки");

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await channel.Talker().Tell(text)).ByKeyOrDefault(
            _ctxKey,
            _ctxName
        );
    }
}
