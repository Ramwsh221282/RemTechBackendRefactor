namespace Parsing.SDK.Browsers.PathSources;

public sealed class BrowserPathSourceFromEnvironmentVariables : IBrowserPathSource
{
    private readonly string _optionKey;

    public BrowserPathSourceFromEnvironmentVariables(string optionKey)
    {
        _optionKey = optionKey;
    }
    
    public string Read()
    {
        if (string.IsNullOrWhiteSpace(_optionKey))
            throw new ArgumentException($"""
                                         {nameof(BrowserPathSourceFromEnvironmentVariables)} class expected 
                                         {nameof(_optionKey)} to be not null or empty.
                                         """);
        string? path = Environment.GetEnvironmentVariable(_optionKey);
        return !string.IsNullOrWhiteSpace(path) ? 
            path :
            throw new ArgumentException($"Browser path from environment variable: {_optionKey} is empty.");
    }
}