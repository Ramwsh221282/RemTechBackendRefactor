using System.Text;
using System.Text.RegularExpressions;

namespace Parsing.RabbitMq.PublishVehicle.Extras;

public sealed class SentencesCollection
{
    private List<string[]> _sentences = [];
    public IReadOnlyList<string[]> Sentences => _sentences;

    private static readonly Regex NonWordSpaceRegex = new(@"[^\w\s]", RegexOptions.Compiled);
    private static readonly Regex NewlineRegex = new(@"[\r\n]+", RegexOptions.Compiled);
    private static readonly Regex MultiSpaceRegex = new(@"\s{2,}", RegexOptions.Compiled);
    private static readonly HashSet<string> Prepositions = new(StringComparer.OrdinalIgnoreCase)
    {
        "!",
        "?",
        ":",
        ".",
        ",",
        "(",
        ")",
        "[",
        "]",
        "{",
        "}",
        "и",
        "в",
        "на",
        "по",
        "за",
        "с",
        "у",
        "о",
        "при",
    };

    public void Fill(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;
        _sentences = SplitIntoListOfArrays(text);
    }

    public string FormText()
    {
        StringBuilder sb = new StringBuilder();
        foreach (string[] sentence in _sentences)
        {
            sb.Append(string.Join(' ', sentence)).Append(' ');
        }

        return sb.ToString().Trim();
    }

    private static string PreprocessString(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
        StringBuilder sb = new StringBuilder(input.Length);
        sb.Append(input);
        string result = NonWordSpaceRegex.Replace(sb.ToString(), " ");
        result = NewlineRegex.Replace(result, " ");
        result = MultiSpaceRegex.Replace(result, " ");
        return result.Trim();
    }

    private static List<string[]> SplitIntoListOfArrays(string input)
    {
        List<string[]> result = new List<string[]>();
        List<string> currentSentence = new List<string>();
        string[] words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        bool numberSeen = false;
        foreach (var word in words)
        {
            if (string.IsNullOrWhiteSpace(word) || IsPreposition(word))
                continue;
            if (IsNumber(word))
            {
                currentSentence.Add(word);
                if (!numberSeen)
                    numberSeen = true;
                continue;
            }
            if (numberSeen)
            {
                AddTrimmedSentence(currentSentence, result);
                currentSentence.Clear();
                numberSeen = false;
                continue;
            }
            currentSentence.Add(word);
        }

        if (currentSentence.Count > 0)
        {
            AddTrimmedSentence(currentSentence, result);
        }

        return result;
    }

    static void AddTrimmedSentence(List<string> sentence, List<string[]> result)
    {
        if (sentence.Count == 0)
            return;
        bool hasNumbers = false;
        for (int i = 0; i < sentence.Count; i++)
        {
            if (IsNumber(sentence[i]))
            {
                hasNumbers = true;
                break;
            }
        }
        if (!hasNumbers)
            return;
        var wordIndices = new List<int>();
        var numberIndices = new List<int>();
        for (int i = 0; i < sentence.Count; i++)
        {
            if (IsNumber(sentence[i]))
                numberIndices.Add(i);
            else
                wordIndices.Add(i);
        }
        if (wordIndices.Count >= 8)
        {
            wordIndices = wordIndices.GetRange(wordIndices.Count - 5, 5);
        }
        var allIndices = new List<int>(wordIndices.Count + numberIndices.Count);
        allIndices.AddRange(wordIndices);
        allIndices.AddRange(numberIndices);
        allIndices.Sort();
        var resultArray = new string[allIndices.Count];
        for (int i = 0; i < allIndices.Count; i++)
        {
            resultArray[i] = sentence[allIndices[i]];
        }

        result.Add(resultArray);
    }

    private static bool IsPreposition(string word)
    {
        return Prepositions.Contains(word);
    }

    private static bool IsNumber(string word)
    {
        if (string.IsNullOrEmpty(word) || word.Length > 10)
            return false;
        return int.TryParse(word, out _);
    }
}
