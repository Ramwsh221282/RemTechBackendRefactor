namespace Parsing.Vehicles.Grpc.Recognition.Weight;

public sealed class OnlyDigitsWeightRecognition : IWeightRecognition
{
    private readonly IWeightRecognition _origin;

    public OnlyDigitsWeightRecognition(IWeightRecognition origin)
    {
        _origin = origin;
    }
    
    public async Task<Characteristic> Recognize(string text)
    {
        Characteristic recognized = await _origin.Recognize(text);
        return !recognized ? 
            recognized :
            new Characteristic(recognized.ReadName(), new OnlyDigitsString(recognized.ReadValue()));
    }
}