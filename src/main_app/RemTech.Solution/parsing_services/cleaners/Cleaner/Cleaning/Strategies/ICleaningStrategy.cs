using PuppeteerSharp;

namespace Cleaner.Cleaning.Strategies;

internal interface ICleaningStrategy
{
    Task Process(IPage page);
}
