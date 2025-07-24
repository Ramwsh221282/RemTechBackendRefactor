using System.Globalization;
using System.Text.RegularExpressions;
using Parsing.Vehicles.Grpc.Recognition.Measurements;
using RemTech.Core.Shared.Primitives;

namespace Parsing.Vehicles.Grpc.Recognition.BucketCapacity;

public sealed partial class MeasuringBucketCapacityRecognition : IBucketCapacityRecognition
{
    private readonly IBucketCapacityRecognition _origin;
    private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.IgnoreCase;
    private readonly Regex _floatingNumberRegex = FloatingNumberRegex();
    
    public MeasuringBucketCapacityRecognition(IBucketCapacityRecognition origin)
    {
        _origin = origin;
    }
    
    public async Task<Characteristic> Recognize(string text)
    {
        Characteristic ctx = await _origin.Recognize(text);
        if (!ctx) return ctx;
        string value = ctx.ReadValue();
        Match match = FloatingNumberRegex().Match(value);
        if (match.Success)
        {
            double doubleValue = Double.Parse(match.Groups[1].Value);
            return new Characteristic(ctx.ReadName(), doubleValue.ToString(CultureInfo.InvariantCulture), new LitresMeasurement());
        }
        return new Characteristic(ctx.ReadName(), new OnlyDigitsString(value).Read(), new LitresMeasurement());
    }
    
    [GeneratedRegex(@"(\d+(?:[.,]\d+))", RegexOptions.IgnoreCase | RegexOptions.Compiled, "ru-RU")]
    private static partial Regex FloatingNumberRegex();
}