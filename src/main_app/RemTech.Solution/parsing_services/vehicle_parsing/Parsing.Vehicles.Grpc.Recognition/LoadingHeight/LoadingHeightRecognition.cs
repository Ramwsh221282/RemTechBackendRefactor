using Recognizer;

namespace Parsing.Vehicles.Grpc.Recognition.LoadingHeight;

public sealed class LoadingHeightRecognition(ICommunicationChannel channel)
    : ILoadingHeightRecognition
{
    private readonly string _ctxKey = string.Intern("LOADING_HEIGHT");
    private readonly string _ctxName = string.Intern("Высота подъёма");

    public async Task<Characteristic> Recognize(string text)
    {
        RecognizeResponse response = await channel.Talker().Tell(text);
        RecognizedCharacteristic recognized = new RecognizedCharacteristic(response);
        Characteristic characteristic = recognized.ByKeyOrDefault(_ctxKey, _ctxName);
        string value = characteristic.ReadValue();
        string onlyDigits = new string(value.Where(char.IsDigit).ToArray());
        return string.IsNullOrWhiteSpace(onlyDigits)
            ? new Characteristic()
            : new Characteristic(_ctxName, onlyDigits);
    }
}
