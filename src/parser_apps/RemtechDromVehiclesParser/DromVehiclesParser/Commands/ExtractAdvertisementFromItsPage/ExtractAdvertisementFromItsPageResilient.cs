using DromVehiclesParser.Parsing.CatalogueParsing.Models;
using DromVehiclesParser.Parsing.ConcreteItemParsing.Models;
using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace DromVehiclesParser.Commands.ExtractAdvertisementFromItsPage;

public sealed class ExtractAdvertisementFromItsPageResilient(
    BrowserManager manager, 
    IPage page, 
    IExtractAdvertisementFromItsPageCommand inner, 
    int attempts = 5) : IExtractAdvertisementFromItsPageCommand
{
    public async Task<DromAdvertisementFromPage> Extract(DromCatalogueAdvertisement catalogueAdvertisement)
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
                return await inner.Extract(catalogueAdvertisement);
            }
            catch (WithdrawException) when (currentAttempt < attempts)
            {
                currentAttempt++;
            }
        }
        
        throw new InvalidOperationException("Failed to extract advertisement from page");
    }
}