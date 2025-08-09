using System.Text.RegularExpressions;

namespace RemTech.Vehicles.Module.Utilities;

internal sealed class StringForVectorStoring(string input)
{
    public string Read()
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        string result = Regex.Replace(input, @"[^\w\s]", " ");
        result = Regex.Replace(result, @"[\r\n]+", " ");
        result = Regex.Replace(result, @"\s{2,}", " ");
        result = result.Trim();
        return result;
    }
}
