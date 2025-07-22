namespace Parsing.SDK.Logging;

public sealed class ConsoleParsingLog  : IParsingLog
{
    public void Info(string info, params object[] args)
    {
        string formatted =  string.Format(info, args);
        Console.WriteLine($"INFO: {formatted}");
    }

    public void Warning(string info, params object[] args)
    {
        string formatted = string.Format(info, args);
        Console.WriteLine($"WARNING: {formatted}");
    }

    public void Error(string info, params object[] args)
    {
        string formatted = string.Format(info, args);
        Console.WriteLine($"ERROR: {formatted}");
    }
}