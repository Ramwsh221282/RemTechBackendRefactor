using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using Parsing.Vehicles.Grpc.Recognition.Measurements;

namespace Parsing.Vehicles.Grpc.Recognition.EnginePower;

public sealed partial class MeasuringEnginePowerRecognition : IEnginePowerRecognition
{
    private readonly IEnginePowerRecognition _origin;

    private const RegexOptions Options =
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

    private const double LsModifier = 1.36;

    private readonly Regex _lsRegex = LsRegex();
    private readonly Regex _kvtRegex = KvtRegex();

    public MeasuringEnginePowerRecognition(IEnginePowerRecognition origin)
    {
        _origin = origin;
    }
    
    public async Task<Characteristic> Recognize(string text)
    {
        Characteristic ctx = await _origin.Recognize(text);
        string value = ctx.ReadValue();
        if (MatchesLs(value, out string lsValue))
            return new Characteristic(ctx.ReadName(), lsValue, new LsMeasurement());
        if (MatchesKvt(value, out string kvtValue))
            return new Characteristic(value, kvtValue, new LsMeasurement());
        return new Characteristic(string.Empty, string.Empty);
    }

    private bool MatchesKvt(string input, [NotNullWhen(true)] out string? value)
    {
        Match match = _kvtRegex.Match(input);
        if (match.Success)
        {
            double doubleValue = double.Parse(match.Groups[1].Value);
            double ls = doubleValue * LsModifier;
            value = ls.ToString(CultureInfo.InvariantCulture);
            return true;
        }
        value = null;
        return false;
    }
    
    private bool MatchesLs(string input, [NotNullWhen(true)] out string? value)
    {
        Match match = _lsRegex.Match(input);
        if (match.Success)
        {
            value = match.Groups[1].Value;
            return true;
        }
        value = null;
        return false;
    }

    [GeneratedRegex(@"(\d+(?:[.,]\d+)?)\s*(?:л\.?\s*с\.?|лс|л\.с\.|hp|HP)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex LsRegex();
    [GeneratedRegex(@"(\d+(?:[.,]\d+)?)\s*(?:к\.?\s*вт\.?|квт)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex KvtRegex();
}