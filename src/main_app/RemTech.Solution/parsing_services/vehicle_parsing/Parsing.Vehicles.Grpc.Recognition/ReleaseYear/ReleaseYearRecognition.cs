using Recognizer;

namespace Parsing.Vehicles.Grpc.Recognition.ReleaseYear;

public sealed class ReleaseYearRecognition(ICommunicationChannel channel) : IReleaseYearRecognition
{
    private readonly string _ctxKey = string.Intern("RELEASE_YEAR");
    private readonly string _ctxName = string.Intern("Год выпуска");

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
