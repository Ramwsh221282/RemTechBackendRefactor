using Recognizer;

namespace Parsing.Vehicles.Grpc.Recognition.TransportHours;

public sealed class TransportHoursRecognition(ICommunicationChannel channel)
    : ITransportHoursRecognition
{
    private readonly string _ctxKey = string.Intern("TRANSPORT_HOURS");
    private readonly string _ctxName = string.Intern("Моточасы");

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
