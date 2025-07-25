using PuppeteerSharp;

namespace Parsing.SDK.Browsers.InstantiationModes;

/// <summary>
/// Interface for declaring different instantiation mode for different executing environments.
/// EG: instantiate browser by browser path from environment variables,
/// or instantiate browser directly from library methods for development purposes.
/// </summary>
public interface IScrapingBrowserInstantiation
{
    /// <summary>
    /// Returns IBrowser instance.
    /// </summary>
    /// <returns>IBrowser instance</returns>
    Task<IBrowser> Instantiation();
}