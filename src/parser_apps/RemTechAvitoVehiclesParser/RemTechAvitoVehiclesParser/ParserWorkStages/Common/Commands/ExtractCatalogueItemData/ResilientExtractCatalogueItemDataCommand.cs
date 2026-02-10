using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.Common.Commands.ExtractCatalogueItemData;

public sealed class ResilientExtractCatalogueItemDataCommand(
    IPage page, 
    BrowserManager manager,
    Serilog.ILogger logger,
    IExtractCatalogueItemDataCommand inner, 
    int retryAmount = 5)
    : IExtractCatalogueItemDataCommand
{
    private int RetryCount { get; } = retryAmount;
    
    private Serilog.ILogger Logger { get; } = logger.ForContext<IExtractCatalogueItemDataCommand>();

    public async Task<AvitoVehicle[]> Handle()
    {
        int currentAttempt = 0;
        while (currentAttempt < RetryCount)
        {
            if (currentAttempt >= RetryCount)
            {
                break;
            }
            
            try
            {
                return await inner.Handle();
            }
            catch (Exception) 
            {
                currentAttempt++;
                page = await manager.RecreatePage(page);
                manager.ReleasePageUsedMemoryResources();
                Logger.Warning("Error extracting catalogue item data. Attempt {Attempt} of {TotalAttempts}.", currentAttempt, RetryCount);
            }
        }
        
        throw new InvalidOperationException("Failed to extract catalogue item data");
    }
}