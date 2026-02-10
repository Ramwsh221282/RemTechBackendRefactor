using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.Common.Commands.ExtractConcreteItem;

public sealed class ResilientExtractConcreteItemCommand(
    IPage page, 
    BrowserManager manager, 
    IExtractConcreteItemCommand inner, 
    int retryAmount = 5)
    : IExtractConcreteItemCommand
{
    public async Task<AvitoVehicle> Handle()
    {
        int currentRetry = 0;
        while (true)
        {
            try
            {
                return await inner.Handle();
            }
            catch (Exception) when (currentRetry < retryAmount)
            {
                currentRetry++;
                page = await manager.RecreatePage(page);
                manager.ReleasePageUsedMemoryResources();
            }
        }
    }
}