using Parsing.Vehicles.Grpc.Recognition.Weight;

namespace Parsing.Vehicles.Grpc.Recognition.LoadingWeight;

public sealed class OnlyDigitsLoadingWeightRecognition : ILoadingWeightRecognition
{
    private readonly ILoadingWeightRecognition _origin;

    public OnlyDigitsLoadingWeightRecognition(ILoadingWeightRecognition origin)
    {
        _origin = origin;
    }

    public async Task<Characteristic> Recognize(string text)
    {
        Characteristic result = await _origin.Recognize(text);
        return !result ? result : new Characteristic(result.ReadName(), new OnlyDigitsString(result.ReadValue()));
    }
}

public sealed class LoadingWeightRecognition : ILoadingWeightRecognition
{
    private readonly CommunicationChannel _channel;
    private readonly string _ctxKey = string.Intern("LOADING_WEIGHT");
    private readonly string _ctxName = string.Intern("Грузоподъёмность");

    public LoadingWeightRecognition(CommunicationChannel channel)
    {
        _channel = channel;
    }

    public async Task<Characteristic> Recognize(string text)
    {
        return new RecognizedCharacteristic(await _channel.Talker().Tell(text)).ByKeyOrDefault(_ctxKey, _ctxName);
    }
}