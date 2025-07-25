namespace Parsing.SDK.Browsers.BrowserLoadings;

/// <summary>
/// Interface for declaring a procedure to load browser on a running machine.
/// </summary>
public interface IBrowserLoading
{
    /// <summary>
    /// Async procedure to load browser on a running machine.
    /// </summary>
    /// <returns>Async procedure to load browser on a running machine.</returns>
    Task LoadBrowser();
}