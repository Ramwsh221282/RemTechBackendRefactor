using Recognizer;

namespace Parsing.Vehicles.Grpc.Recognition.UnloadingHeight;

public sealed class UnloadingHeightRecognition(ICommunicationChannel channel)
    : IUnloadingHeightRecognition
{
    private readonly string _ctxKey = string.Intern("UNLOADING_HEIGHT");
    private readonly string _ctxName = string.Intern("Высота выгрузки");

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
