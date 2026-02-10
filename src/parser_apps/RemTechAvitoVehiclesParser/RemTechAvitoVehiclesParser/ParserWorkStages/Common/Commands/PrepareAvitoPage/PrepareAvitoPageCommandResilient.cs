using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.Common.Commands.PrepareAvitoPage;

public sealed class PrepareAvitoPageCommandResilient(
    BrowserManager manager, 
    IPage page, 
    IPrepareAvitoPageCommand inner, 
    int attemptsCount = 5) : IPrepareAvitoPageCommand
{
    public async Task Handle()
    {
        int currentAttempt = 0;
        while (currentAttempt < attemptsCount)
        {
            if (currentAttempt >= attemptsCount)
            {
                break;
            }
            
            try
            {
                await inner.Handle();
                return;
            }
            catch (Exception e) when (currentAttempt < attemptsCount)
            {
                currentAttempt++;
                page = await manager.RecreatePage(page);
                manager.ReleasePageUsedMemoryResources();
            }
        }
        
        throw new InvalidOperationException("Failed to prepare avito page");
    }
}