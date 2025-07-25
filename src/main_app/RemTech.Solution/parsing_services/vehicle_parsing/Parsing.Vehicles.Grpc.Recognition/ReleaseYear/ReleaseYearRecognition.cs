namespace Parsing.Vehicles.Grpc.Recognition.ReleaseYear;

public sealed class ReleaseYearRecognition : IReleaseYearRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = string.Intern("RELEASE_YEAR");
    private readonly string _ctxName = string.Intern("Год выпуска");

    public ReleaseYearRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }
    
    public async Task<Characteristic> Recognize(string text)
    {
        return  new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxName);
    }
}