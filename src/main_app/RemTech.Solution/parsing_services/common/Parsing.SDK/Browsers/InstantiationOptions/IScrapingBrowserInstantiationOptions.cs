using PuppeteerSharp;

namespace Parsing.SDK.Browsers.InstantiationOptions;

/// <summary>
/// Options source that declares different modes of browser instatiation.
/// </summary>
public interface IScrapingBrowserInstantiationOptions
{
    /// <summary>
    /// Options that declare different modes for browser instantiation.
    /// </summary>
    /// <returns>Browser instantiation options.</returns>
    LaunchOptions Configured();
}