using DromVehiclesParser.Parsing.CatalogueParsing.Models;
using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace DromVehiclesParser.Commands.HoverCatalogueImages;

public sealed class HoverAdvertisementsCatalogueImagesResilient(
    BrowserManager manager, 
    IPage webPage, 
    IHoverAdvertisementsCatalogueImagesCommand inner,
    int attempts = 5)
    : IHoverAdvertisementsCatalogueImagesCommand
{
    public async Task Hover(DromCataloguePage dromCataloguePage)
    {
        int currentAttempt = 0;
        while (true)
        {
            if (currentAttempt > 0)
            {
                break;
            }
            
            try
            {
                await inner.Hover(dromCataloguePage);
                return;
            }
            catch (Exception ex) when (currentAttempt < attempts)
            {
                webPage = await manager.RecreatePage(webPage);
                manager.ReleasePageUsedMemoryResources();
                currentAttempt++;
            }
        }
        
        throw new InvalidOperationException("Failed to hover catalogue images");
    }
}