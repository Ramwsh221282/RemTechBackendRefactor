using Parsing.Vehicles.Grpc.Recognition.Measurements;

namespace Parsing.Vehicles.Grpc.Recognition.UnloadingHeight;

public sealed class MeasuringUnloadingHeightRecognition : IUnloadingHeightRecognition
{
    private readonly IUnloadingHeightRecognition _origin;

    public MeasuringUnloadingHeightRecognition(IUnloadingHeightRecognition origin)
    {
        _origin = origin;
    }
    
    public async Task<Characteristic> Recognize(string text)
    {
        Characteristic ctx = await _origin.Recognize(text);
        return !ctx ? ctx : new Characteristic(ctx.ReadName(), ctx.ReadValue(), new MillimetersMeasurement());
    }
}