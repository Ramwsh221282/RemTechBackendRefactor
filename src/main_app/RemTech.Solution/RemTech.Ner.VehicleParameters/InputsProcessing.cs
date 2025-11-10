using System.Text.RegularExpressions;

namespace RemTech.Ner.VehicleParameters;

internal static partial class InputsProcessing
{
    public static InputWords GetWords(string inputText)
    {
        return new InputWords(
            inputText.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
        );
    }
    
    private static readonly Regex _wordRegex = WordRegex();
    
    [GeneratedRegex(@"\b\w+\b", RegexOptions.Compiled)]
    private static partial Regex WordRegex();
}