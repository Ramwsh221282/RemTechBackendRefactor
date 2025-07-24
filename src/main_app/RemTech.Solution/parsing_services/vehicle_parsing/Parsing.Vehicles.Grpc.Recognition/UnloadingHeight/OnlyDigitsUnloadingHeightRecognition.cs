namespace Parsing.Vehicles.Grpc.Recognition.UnloadingHeight;

public sealed class OnlyDigitsUnloadingHeightRecognition : IUnloadingHeightRecognition
{
    private readonly IUnloadingHeightRecognition _origin;

    public OnlyDigitsUnloadingHeightRecognition(IUnloadingHeightRecognition origin)
    {
        _origin = origin;
    }
    
    public async Task<Characteristic> Recognize(string text)
    {
        Characteristic ctx = await _origin.Recognize(text);
        if (!ctx)
            return ctx;
        string value = ctx.ReadValue();
        string onlyDigitsValue = new string(value.Where(char.IsDigit).ToArray());
        return new Characteristic(ctx.ReadName(), onlyDigitsValue);
    }
}