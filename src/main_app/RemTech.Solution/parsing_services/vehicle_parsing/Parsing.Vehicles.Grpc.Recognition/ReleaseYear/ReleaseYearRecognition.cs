namespace Parsing.Vehicles.Grpc.Recognition.ReleaseYear;

public sealed class ReleaseYearRecognition(ICommunicationChannel channel) : IReleaseYearRecognition
{
    private readonly string _ctxKey = string.Intern("RELEASE_YEAR");
    private readonly string _ctxName = string.Intern("Год выпуска");

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await channel.Talker().Tell(text)).ByKeyOrDefault(
            _ctxKey,
            _ctxName
        );
    }
}
