using Recognizer;

namespace Parsing.Vehicles.Grpc.Recognition.Weight;

public sealed class WeightRecognition(ICommunicationChannel channel) : IWeightRecognition
{
    private readonly string _ctxKey = string.Intern("WEIGHT");
    private readonly string _ctxName = string.Intern("Эксплуатационная масса");

    public async Task<Characteristic> Recognize(string text)
    {
        RecognizeResponse response = await channel.Talker().Tell(text);
        RecognizedCharacteristic recognized = new RecognizedCharacteristic(response);
        Characteristic characteristic = recognized.ByKeyOrDefault(_ctxKey, _ctxName);
        string value = characteristic.ReadValue();
        string onlyDigits = new string(value.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(onlyDigits))
            return new Characteristic();
        if (onlyDigits.Length == 2 || onlyDigits.Length == 1)
            return new Characteristic(_ctxName, (int.Parse(onlyDigits) * 1000).ToString());
        return new Characteristic(_ctxName, onlyDigits);
    }
}
