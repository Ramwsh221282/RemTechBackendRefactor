using Recognizer;

namespace Parsing.Vehicles.Grpc.Recognition;

public sealed class RecognizedCharacteristic
{
    private readonly RecognizeResponse _response;

    public RecognizedCharacteristic(RecognizeResponse response)
    {
        _response = response;
    }

    public Characteristic ByKeyOrDefault(string key, string name)
    {
        Recognitions? recognition = _response.Results.FirstOrDefault(r => r.RecognizedEntity == key);
        return recognition == null ? new Characteristic() : new Characteristic(name, recognition.RecognizedTextChunk);
    }
}