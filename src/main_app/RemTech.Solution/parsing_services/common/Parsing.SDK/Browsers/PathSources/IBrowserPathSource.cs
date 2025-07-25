namespace Parsing.SDK.Browsers.PathSources;

/// <summary>
/// Source of browser executable path. This may be ENV source, or raw text file path source.
/// </summary>
public interface IBrowserPathSource
{
    /// <summary>
    /// Browser executable source.
    /// </summary>
    /// <returns>Browser executable source.</returns>
    string Read();
}